using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Egelke.Eid.SmartCard.WinScard;

namespace Egelke.Eid.Client
{
    public class Readers : IDisposable
    {

	    private static char[] EID_CARD_NAME = { 'B', 'e', 'i', 'd', '\0', '\0' };

        private SafeCardContextHandle context;

        public Readers(ReaderScope scope)
        {
            int retVal;
            switch (scope)
            {
                case ReaderScope.Null:
                    context = new SafeCardContextHandle(IntPtr.Zero);
                    break;
                case ReaderScope.System:
                    retVal = NativeMethods.SCardEstablishContext(ContextScope.System, IntPtr.Zero, IntPtr.Zero, out context);
                    if (retVal != 0) throw new InvalidOperationException("Failed to create static context for reader: 0x" + retVal.ToString("X"));
                    break;
                case ReaderScope.User:
                    retVal = NativeMethods.SCardEstablishContext(ContextScope.User, IntPtr.Zero, IntPtr.Zero, out context);
                    if (retVal != 0) throw new InvalidOperationException("Failed to create static context for reader: 0x" + retVal.ToString("X"));
                    break;
            }
        }

        ~Readers()
        {
            Dispose(false);
        }

        public event EventHandler<EventArgs> EidCardRequest;

        public event EventHandler<EventArgs> EidCardRequestCancellation;

        public List<String> Names
        {
            get
            {
                int retVal;
                int size = 0;
                char[] names = null;

                retVal = NativeMethods.SCardListReaders(context, null, names, ref size);
                if (retVal == 0x8010002E) return null;
                if (retVal != 0) throw new InvalidOperationException("Failed to list readers (length): 0x" + retVal.ToString("X"));

                names = new Char[size];
                retVal = NativeMethods.SCardListReaders(context, null, names, ref size);
                if (retVal != 0) throw new InvalidOperationException("Failed to list readers: 0x" + retVal.ToString("X"));

                return names.ToStringList();
            }
        }

        public EidCard WaitForEid(TimeSpan timeout)
        {
            int retVal;
            bool requestedEid = false;
            bool wrongCard = false;
            DateTime start = DateTime.Now;

            //No readers, so nothing to find
            List<String> names = this.Names;
            if (names.Count == 0) return null;

            //Make a list of all readers in scope
            var readerStates = new List<SCARD_READERSTATE>();
            foreach (String name in names)
            {
                SCARD_READERSTATE readerState = new SCARD_READERSTATE();
                readerState.szReader = name;
                readerState.dwCurrentState = ReaderState.SCARD_STATE_UNKNOWN;
                readerStates.Add(readerState);
            }
            SCARD_READERSTATE[] readerStateArray = readerStates.ToArray();

            while (true)
            {
                //Lets see if there is an eID is available in one of the readers
                retVal = NativeMethods.SCardLocateCards(context, "Beid", readerStateArray, readerStateArray.Length);
                if (retVal != 0) throw new InvalidOperationException("Failed to locate card: 0x" + retVal.ToString("X"));

                //check if one of the readers has the eID indicator, update the status in the mean time
                for(int i=0; i<readerStateArray.Length; i++)
                {
                    if ((readerStateArray[i].dwEventState & ReaderState.SCARD_STATE_ATRMATCH) == ReaderState.SCARD_STATE_ATRMATCH)
                    {
                        if (requestedEid) OnEidCardRequestCancellation();
                        if (wrongCard) Thread.Sleep(1000); //there was a wrong card, give the new one some time (if you don't it fails)
                        return new  EidCard(this.context, readerStateArray[i].szReader); //found, return open reader
                    }
                    readerStateArray[i].dwCurrentState = readerStateArray[i].dwEventState;
                }

                if (!requestedEid)
                {
                    OnEidCardRequest();
                    requestedEid = true;
                }
                else
                {
                    wrongCard = true;
                }

                //calculate how much time should be waited
                TimeSpan remainingTimeout = timeout - (DateTime.Now - start);
                if (remainingTimeout < TimeSpan.Zero) throw new TimeoutException("No Beid found in time"); ; //safety check

                //wait until the status change before retrying
                retVal = NativeMethods.SCardGetStatusChange(context, Convert.ToInt32(remainingTimeout.TotalMilliseconds), readerStateArray, readerStateArray.Length);
                if (retVal == 0x8010000A) throw new TimeoutException("No Beid found in time");
                if (retVal != 0) throw new InvalidOperationException("Failed get wait for status change: 0x" + retVal.ToString("X"));

                //check if we need to do a little wait
                for (int i = 0; i < readerStateArray.Length; i++)
                {
                    readerStateArray[i].dwCurrentState = readerStateArray[i].dwEventState;
                }
            }
        }

        protected virtual void OnEidCardRequest()
        {
            if (EidCardRequest != null)
            {
                EidCardRequest(this, new EventArgs());
            }
        }

        protected virtual void OnEidCardRequestCancellation()
        {
            if (EidCardRequestCancellation != null)
            {
                EidCardRequestCancellation(this, new EventArgs());
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool managed)
        {
            if (managed)
            {
                if (!context.IsClosed && !context.IsInvalid) context.Close();
            }
        }
    }
}
