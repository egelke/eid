using System;
using System.Collections.Generic;
using System.Text;

namespace Egelke.Eid.Client.SCard
{
    public class NativeEventArgs : EventArgs
    {
        public uint ErrorCode { get; private set; }

        public NativeEventArgs() : this(0)
        {

        }

        public NativeEventArgs(uint errorCode)
        {
            ErrorCode = errorCode;
        }
    }
}
