/*
 *  This file is part of .Net eID Client.
 *  Copyright (C) 2014-2019 Egelke BVBA
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
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Egelke.Eid.Client
{
    [BestFitMapping(false, ThrowOnUnmappableChar = true)]
    internal static class NativeMethods
    {
        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SCardEstablishContext([MarshalAs(UnmanagedType.U4)] ContextScope dwScope, IntPtr pvReserved1, IntPtr pvReserved2, out CardContextSafeHandler phContext);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SCardReleaseContext(IntPtr hContext);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SCardListReaders(CardContextSafeHandler hContext, [In, MarshalAs(UnmanagedType.LPArray)] Char[] mszGroups, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] Char[] mszReaders, [In, Out] ref int pcchReaders);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SCardLocateCards(CardContextSafeHandler hContext, [In, MarshalAs(UnmanagedType.LPArray)] Char[] mszCards, [In, Out] SCARD_READERSTATE[] rgReaderStates, int cReaders);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SCardListCards(CardContextSafeHandler hContext, byte[] pbAtr, IntPtr rgguidInterfaces, int cguidInterfaceCount, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] Char[] mszCards, [In, Out] ref int pcchCards);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SCardGetStatusChange(CardContextSafeHandler hContext, int dwTimeout, [In, Out] SCARD_READERSTATE[] rgReaderStates, int cReaders);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SCardConnect(CardContextSafeHandler hContext, String szReader, CardShareMode dwShareMode, CardProtocols dwPreferredProtocols, out CardSafeHandler phCard, out CardProtocols pdwActiveProtocol);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SCardDisconnect(IntPtr hCard, CardDisposition dwDisposition);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SCardGetAttrib(CardSafeHandler hCard, CardAttrId dwAttrId, IntPtr pbAttr, ref int pcbAttrLen);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SCardBeginTransaction(CardSafeHandler hCard);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SCardEndTransaction(CardSafeHandler hCard, CardDisposition dwDisposition);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SCardTransmit(CardSafeHandler hCard, SCARD_IO_REQUEST pioSendPci, byte[] pbSendBuffer, int cbSendLength, [In, Out] SCARD_IO_REQUEST pioRecvPci, [Out] byte[] pbRecvBuffer, [In, Out] ref int pcbRecvLength);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        internal static extern uint SCardFreeMemory(CardContextSafeHandler hContext, IntPtr pvMem);
    }
}
