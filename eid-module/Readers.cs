using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Egelke.Eid.Client
{
    public class Readers : IDisposable
    {
        private CardContextSafeHandler context;

        private bool run;

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

            int size = 0;

            //List the readers
            //todo::listen for new readers changes (via the "\\?PnP?\Notification" reader)
            char[] readers = null;
            retVal = NativeMethods.SCardListReaders(context, null, readers, ref size);
            if (retVal != 0 && retVal != 0x8010002E) throw new InvalidOperationException("Failed to list readers (length): 0x" + retVal.ToString("X"));

            if (retVal == 0x8010002E) //no readers
            {
                List = new List<string>(0);
            }
            else {
                readers = new char[size];
                retVal = NativeMethods.SCardListReaders(context, null, readers, ref size);
                if (retVal != 0) throw new InvalidOperationException("Failed to list readers: 0x" + retVal.ToString("X"));

                List = MultiString.ToStringList(readers);
            }

            //check for status updates
#if NET20
            var bgt = new System.Threading.Thread(DetectChanges);
            bgt.Start();
#else
            var bgt = System.Threading.Tasks.Task.Factory.StartNew(DetectChanges);
#endif
        }



        ~Readers()
        {
            Dispose(false);
        }

        public event EventHandler<CardEventArgs> CardInsert;

        public List<Card> ListCards()
        {
            List<SCARD_READERSTATE> readerStateList = new List<SCARD_READERSTATE>();
            foreach (String name in List)
            {
                var readerState = new SCARD_READERSTATE()
                {
                    szReader = name,
                    dwCurrentState = ReaderState.SCARD_STATE_UNKNOWN, 
                };
                 readerStateList.Add(readerState);
            }
            SCARD_READERSTATE[] readerStates = readerStateList.ToArray();

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

        public List<Card> ListCards(params string[] cardNames)
        {
            List<SCARD_READERSTATE> readerStateList = new List<SCARD_READERSTATE>();
            foreach (String name in List)
            {
                var readerState = new SCARD_READERSTATE()
                {
                    szReader = name,
                    dwCurrentState = ReaderState.SCARD_STATE_UNKNOWN,
                };
                readerStateList.Add(readerState);
            }
            SCARD_READERSTATE[] readerStates = readerStateList.ToArray();

            List<Card> cards = new List<Card>();
            uint retVal = NativeMethods.SCardLocateCards(context, MultiString.ToMultiString(cardNames), readerStates, readerStates.Length);
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

            //Prepare the reader state for the first usage.
            List<SCARD_READERSTATE> readerStateList = new List<SCARD_READERSTATE>();
            foreach (String name in List)
            {
                var readerState = new SCARD_READERSTATE()
                {
                    szReader = name,
                    dwCurrentState = ReaderState.SCARD_STATE_UNKNOWN,
                };
                readerStateList.Add(readerState);
            }
            SCARD_READERSTATE[] readerStates = readerStateList.ToArray();

            //Init the status
            retVal = NativeMethods.SCardGetStatusChange(context, 0, readerStates, readerStates.Length);
            if (retVal != 0) throw new InvalidOperationException("Failed to update the status: 0x" + retVal.ToString("X"));

            run = true;
            while (run)
            {
                //mark the received info as known
                for (int i = 0; i < readerStates.Length; i++)
                {
                    readerStates[i].dwCurrentState = readerStates[i].dwEventState;
                }
                retVal = NativeMethods.SCardGetStatusChange(context, 5000, readerStates, readerStates.Length);
                if (retVal == 0x8010000A) continue; //timeout
                if (retVal != 0) throw new InvalidOperationException("Failed to update the status: 0x" + retVal.ToString("X"));
               
                //process the new info
                for (int i = 0; i < readerStates.Length; i++)
                {
                    SCARD_READERSTATE state = readerStates[i];
                    if ((state.dwEventState & ReaderState.SCARD_STATE_CHANGED) == ReaderState.SCARD_STATE_CHANGED)
                    {
                        //check for insert
                        if ((state.dwCurrentState & ReaderState.SCARD_STATE_EMPTY) == ReaderState.SCARD_STATE_EMPTY
                            && (state.dwEventState & ReaderState.SCARD_STATE_PRESENT) == ReaderState.SCARD_STATE_PRESENT)
                        {
                            OnCardInsert(CreateCard(state));
                        }
                    }
                }
            }
        }

        private Card CreateCard(SCARD_READERSTATE readerstate)
        {
            uint retVal;

            if ((readerstate.dwCurrentState & ReaderState.SCARD_STATE_PRESENT) == ReaderState.SCARD_STATE_PRESENT)
                throw new ArgumentException("No card is present in the reader");

            int cardNamesLen = 0;
            char[] cardNames = null;
            retVal = NativeMethods.SCardListCards(context, readerstate.rgbAtr, IntPtr.Zero, 0, cardNames, ref cardNamesLen);
            if (retVal != 0) throw new InvalidOperationException("Failed to list card names from ATR (length): 0x" + retVal.ToString("X"));

            cardNames = new char[cardNamesLen];
            retVal = NativeMethods.SCardListCards(context, readerstate.rgbAtr, IntPtr.Zero, 0, cardNames, ref cardNamesLen);
            if (retVal != 0) throw new InvalidOperationException("Failed to list card names from ATR: 0x" + retVal.ToString("X"));

            List<String> carNameList = MultiString.ToStringList(cardNames);
            return EidCard.IsEid(carNameList) ? new EidCard(context, readerstate.szReader, readerstate.rgbAtr) : new Card(context, readerstate.szReader, readerstate.rgbAtr);
        }

        protected virtual void OnCardInsert(Card card) => CardInsert?.Invoke(this, new CardEventArgs(card));

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool managed)
        {
            run = false;
            if (!context.IsClosed && !context.IsInvalid) context.Dispose();
        }
    }
}
