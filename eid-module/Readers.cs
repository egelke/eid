using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;

namespace Egelke.Eid.Client
{
    public class Readers : IDisposable
    {
        private CardContextSafeHandler context;

        private bool run;

        private SCARD_READERSTATE[] readerStates;

#if NET20
       private System.Threading.Thread bgt;
#else
        private System.Threading.Tasks.Task bgt;
#endif

        public List<String> List { get; private set; }

        public Readers(ReaderScope scope)
        {
            uint retVal;
            switch (scope)
            {
                case ReaderScope.Null:
                    context = new CardContextSafeHandler(IntPtr.Zero);
                    break;
                case ReaderScope.System:
                    retVal = NativeMethods.SCardEstablishContext(ContextScope.SCARD_SCOPE_SYSTEM, IntPtr.Zero, IntPtr.Zero, out context);
                    if (retVal != 0) throw new InvalidOperationException("Failed to create static context for reader: 0x" + retVal.ToString("X"));
                    break;
                case ReaderScope.User:
                    retVal = NativeMethods.SCardEstablishContext(ContextScope.SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, out context);
                    if (retVal != 0) throw new InvalidOperationException("Failed to create static context for reader: 0x" + retVal.ToString("X"));
                    break;
            }

            List = new List<string>();
            UpdateList();
        }

        public void StartListen()
        {
            lock (this)
            {
                if (run) return;
                run = true;


                uint retVal;
                UpdateList();

                //Prepare the reader state for the first usage.
                List<SCARD_READERSTATE> readerStateList = List
                    .Select(n => new SCARD_READERSTATE() { szReader = n, dwCurrentState = ReaderState.SCARD_STATE_UNKNOWN })
                    .ToList();
                //Listen for new readers
                readerStateList.Add(new SCARD_READERSTATE() { szReader = @"\\?PnP?\Notification", dwCurrentState = ReaderState.SCARD_STATE_UNKNOWN });
                readerStates = readerStateList.ToArray();

                //Init the status
                retVal = NativeMethods.SCardGetStatusChange(context, 0, readerStates, readerStates.Length);
                if (retVal != 0) throw new InvalidOperationException("Failed to update the status: 0x" + retVal.ToString("X"));

#if NET20
                bgt = new System.Threading.Thread(DetectChanges);
                bgt.Start();
#else
                bgt = System.Threading.Tasks.Task.Factory.StartNew(DetectChanges, System.Threading.Tasks.TaskCreationOptions.LongRunning);
#endif
            }
        }

        public void StopListen()
        {
            lock (this)
            {
                run = false;
#if NET20
                bgt.Join();
#else
                bgt.Wait();
#endif
            }
        }

        ~Readers()
        {
            Dispose(false);
        }

        public event EventHandler<CardEventArgs> CardInsert;
        public event EventHandler<SCard.NativeEventArgs> ListenerStopped;

        public List<Card> ListCards()
        {
            //Listen for status changes.
            SCARD_READERSTATE[] readerStates = List
                .Select(n => new SCARD_READERSTATE() { szReader = n, dwCurrentState = ReaderState.SCARD_STATE_UNKNOWN, })
                .ToArray();

            //Get the status of all readers
            List<Card> cards = new List<Card>();
            uint retVal = NativeMethods.SCardGetStatusChange(context, 0, readerStates, readerStates.Length);
            if (retVal != 0) throw new InvalidOperationException("Failed to update the status: 0x" + retVal.ToString("X"));
            for (int i = 0; i < readerStates.Length; i++)
            {
                SCARD_READERSTATE state = readerStates[i];
                if ((state.dwEventState & ReaderState.SCARD_STATE_PRESENT) == ReaderState.SCARD_STATE_PRESENT)
                {
                    cards.Add(CreateCard(state));
                }
            }
            return cards;
        }

        private void DetectChanges()
        {
            uint retVal;

            run = true;
            while (run)
            {
                //mark the received info as known
                for (int i = 0; i < readerStates.Length; i++)
                {
                    readerStates[i].dwCurrentState = readerStates[i].dwEventState;
                }
                retVal = NativeMethods.SCardGetStatusChange(context, 1000, readerStates, readerStates.Length);
                if (retVal == 0x8010000A) continue; //timeout
                if (retVal != 0)
                {
                    //todo:logging ("Failed to update the status: 0x" + retVal.ToString("X"))
                    OnListenerStopped(retVal);
                    return;
                }

                //process the new info
                bool readerChange = false;
                int oldReaderCount = 0;
                int newReaderCount = 0;
                for (int i = 0; i < readerStates.Length; i++)
                {
                    SCARD_READERSTATE state = readerStates[i];
                    if ((state.dwEventState & ReaderState.SCARD_STATE_CHANGED) == ReaderState.SCARD_STATE_CHANGED)
                    {
                        //check for insert
                        if (((state.dwCurrentState & ReaderState.SCARD_STATE_EMPTY) == ReaderState.SCARD_STATE_EMPTY //It was empty
                                || state.dwCurrentState == ReaderState.SCARD_STATE_UNAWARE //it is a new reader
                            ) && (state.dwEventState & ReaderState.SCARD_STATE_PRESENT) == ReaderState.SCARD_STATE_PRESENT) //there is card present now
                        {
                            OnCardInsert(CreateCard(state));
                        }
                        if (state.szReader == @"\\?PnP?\Notification")
                        {
                            readerChange = true;
                        }
                    }
                    //check for new reader update
                    if (state.szReader == @"\\?PnP?\Notification")
                    {
                        oldReaderCount = (int)state.dwCurrentState >> 16;
                        newReaderCount = (int)state.dwEventState >> 16;
                    }
                   
                }
                if (readerChange)
                {
                    UpdateList();
                    List<SCARD_READERSTATE> readerStateList = List
                        .Select(n => new SCARD_READERSTATE()
                        {
                            szReader = n,
                            dwEventState = readerStates.Where(sr => sr.szReader == n).Select(sr => sr.dwEventState).FirstOrDefault()
                        })
                        .ToList();
                    //Listen for new readers
                    readerStateList.Add(new SCARD_READERSTATE() { szReader = @"\\?PnP?\Notification", dwEventState = (ReaderState) (newReaderCount << 16) });
                    readerStates = readerStateList.ToArray();
                }
            }
            OnListenerStopped(0);
        }

        private Card CreateCard(SCARD_READERSTATE readerstate)
        {
            if ((readerstate.dwEventState & ReaderState.SCARD_STATE_PRESENT) != ReaderState.SCARD_STATE_PRESENT)
                throw new ArgumentException("No card is present in the reader");

            byte[] atr = new byte[readerstate.cbAtr];
            Buffer.BlockCopy(readerstate.rgbAtr, 0, atr, 0, readerstate.cbAtr);
            return EidCard.IsEid(atr) ? new EidCard(context, readerstate.szReader, atr) : new Card(context, readerstate.szReader, atr);
        }

        private void UpdateList()
        {
            uint retVal;
            int size = 0;
            char[] readers = null;
            retVal = NativeMethods.SCardListReaders(context, null, readers, ref size);
            if (retVal != 0 && retVal != 0x8010002E) throw new InvalidOperationException("Failed to list readers (length): 0x" + retVal.ToString("X"));

            if (retVal == 0x8010002E) //no readers
            {
                List.Clear();
            }
            else
            {
                readers = new char[size];
                retVal = NativeMethods.SCardListReaders(context, null, readers, ref size);
                if (retVal != 0) throw new InvalidOperationException("Failed to list readers: 0x" + retVal.ToString("X"));

                List<String> newList = MultiString.ToStringList(readers);
                foreach (String name in List.ToList()) if (!newList.Contains(name)) List.Remove(name);
                foreach (String name in newList) if (!List.Contains(name)) List.Add(name);
            }
        }

        protected virtual void OnCardInsert(Card card) => CardInsert?.Invoke(this, new CardEventArgs(card));

        protected virtual void OnListenerStopped(uint errorCode) => ListenerStopped?.Invoke(this, new SCard.NativeEventArgs(errorCode));

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool managed)
        {
            run = false;
            if (!context.IsClosed && !context.IsInvalid) context.Dispose();
        }
    }
}
