using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egelke.Eid.Client
{
    public class EidReaders : IDisposable
    {
        private CardContextSafeHandler context;

        public EidReaders(ReaderScope scope)
        {
            uint retVal;
            switch (scope)
            {
                case ReaderScope.Null:
                    context = new CardContextSafeHandler(IntPtr.Zero);
                    break;
                case ReaderScope.System:
                    retVal = NativeMethods.SCardEstablishContext(ContextScope.SCARD_SCOPE_SYSTEM, IntPtr.Zero, IntPtr.Zero, out context);
                    if (retVal != 0) throw new InvalidOperationException("Failed to create static context for reader: " + retVal.ToString("X"));
                    break;
                case ReaderScope.User:
                    retVal = NativeMethods.SCardEstablishContext(ContextScope.SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, out context);
                    if (retVal != 0) throw new InvalidOperationException("Failed to create static context for reader: " + retVal.ToString("X"));
                    break;
            }
        }

        ~EidReaders()
        {
            Dispose(false);
        }

        public List<String> Names
        {
            get
            {
                uint retVal;
                int size = 0;
                char[] names = null;

                retVal = NativeMethods.SCardListReaders(context, IntPtr.Zero, names, ref size);
                if (retVal == 0x8010002E) return null;
                if (retVal != 0) throw new InvalidOperationException("Failed to list readers (length): " + retVal.ToString("X"));

                names = new Char[size];
                retVal = NativeMethods.SCardListReaders(context, IntPtr.Zero, names, ref size);
                if (retVal != 0) throw new InvalidOperationException("Failed to list readers: " + retVal.ToString("X"));

                return names.ToStringList();
            }
        }

        public EidReader OpenReader(String name)
        {
            var reader = new EidReader(name);
            reader.Connect();
            return reader;
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
