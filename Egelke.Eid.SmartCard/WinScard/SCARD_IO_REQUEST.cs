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

using System.Runtime.InteropServices;

namespace Egelke.Eid.SmartCard.WinScard
{
    [StructLayout(LayoutKind.Sequential)]
    public class SCARD_IO_REQUEST
    {
	    public static readonly SCARD_IO_REQUEST T0 = new SCARD_IO_REQUEST(ProtocolControlInformation.SCARD_PCI_T0);
        internal static readonly SCARD_IO_REQUEST T1 = new SCARD_IO_REQUEST(ProtocolControlInformation.SCARD_PCI_T1);

        private uint dwProtocol;
        private int cbPciLength;

        private SCARD_IO_REQUEST(ProtocolControlInformation protocol)
        {
            this.dwProtocol = (uint) protocol;
            this.cbPciLength = Marshal.SizeOf(typeof(SCARD_IO_REQUEST));
        }
    }
}
