/*
 *  This file is part of .Net eID Client.
 *  Copyright (C) 2014 Egelke BVBA
 *
 *  .Net eID Client is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 2.1 of the License, or
 *  (at your option) any later version.
 *
 *  .Net eID Client is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with .Net eID Client.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Runtime.InteropServices;

namespace Egelke.Eid.SmartCard.WinScard
{
    [BestFitMapping(false, ThrowOnUnmappableChar = true)]
    public static class NativeMethods
    {
        [DllImport("winscard.dll", CharSet = CharSet.Auto)]
        public static extern uint SCardEstablishContext([MarshalAs(UnmanagedType.U4)] ContextScope dwScope, IntPtr pvReserved1, IntPtr pvReserved2, out SafeCardContextHandle phContext);

        [DllImport("winscard.dll", CharSet = CharSet.Auto)]
        internal static extern uint SCardReleaseContext(IntPtr hContext);

        [DllImport("winscard.dll", CharSet = CharSet.Auto)]
        public static extern uint SCardListReaders(SafeCardContextHandle hContext, [In, MarshalAs(UnmanagedType.LPArray)] Char[] mszGroups, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] Char[] mszReaders, [In, Out] ref int pcchReaders);

        [DllImport("winscard.dll", CharSet = CharSet.Auto)]
        public static extern uint SCardLocateCards(SafeCardContextHandle hContext, [In, MarshalAs(UnmanagedType.LPArray)] Char[] mszCards, [In, Out] SCARD_READERSTATE[] rgReaderStates, int cReaders);

        [DllImport("winscard.dll", CharSet = CharSet.Auto)]
        internal static extern uint SCardListCards(SafeCardContextHandle hContext, byte[] pbAtr, IntPtr rgguidInterfaces, int cguidInterfaceCount, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] Char[] mszCards, [In, Out] ref int pcchCards);

        [DllImport("winscard.dll", CharSet = CharSet.Auto)]
        public static extern uint SCardGetStatusChange(SafeCardContextHandle hContext, int dwTimeout, [In, Out] SCARD_READERSTATE[] rgReaderStates, int cReaders);

        [DllImport("winscard.dll", CharSet = CharSet.Auto)]
        public static extern uint SCardConnect(SafeCardContextHandle hContext, String szReader, CardShareMode dwShareMode, CardProtocols dwPreferredProtocols, out SafeCardHandler phCard, out CardProtocols pdwActiveProtocol);

        [DllImport("winscard.dll", CharSet = CharSet.Auto)]
        internal static extern uint SCardDisconnect(IntPtr hCard, CardDisposition dwDisposition);

        [DllImport("winscard.dll", CharSet = CharSet.Auto)]
        internal static extern uint SCardGetAttrib(SafeCardHandler hCard, CardAttrId dwAttrId, IntPtr pbAttr, ref int pcbAttrLen);

        [DllImport("winscard.dll", CharSet = CharSet.Auto)]
        public static extern uint SCardBeginTransaction(SafeCardHandler hCard);

        [DllImport("winscard.dll", CharSet = CharSet.Auto)]
        public static extern uint SCardEndTransaction(SafeCardHandler hCard, CardDisposition dwDisposition);

        [DllImport("winscard.dll", CharSet = CharSet.Auto)]
        public static extern uint SCardTransmit(SafeCardHandler hCard, SCARD_IO_REQUEST pioSendPci, byte[] pbSendBuffer, int cbSendLength, [In, Out] SCARD_IO_REQUEST pioRecvPci, [Out] byte[] pbRecvBuffer, [In, Out] ref int pcbRecvLength);

        [DllImport("winscard.dll", CharSet = CharSet.Auto)]
        internal static extern uint SCardFreeMemory(SafeCardContextHandle hContext, IntPtr pvMem);
    }
}
