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
    [StructLayout(LayoutKind.Sequential)]
    internal class SCARD_IO_REQUEST
    {
        internal static readonly SCARD_IO_REQUEST T0 = new SCARD_IO_REQUEST(CardPCI.SCARD_PCI_T0);
        internal static readonly SCARD_IO_REQUEST T1 = new SCARD_IO_REQUEST(CardPCI.SCARD_PCI_T1);

        private readonly uint dwProtocol;
        private readonly int cbPciLength;

        private SCARD_IO_REQUEST(CardPCI protocol)
        {
            this.dwProtocol = (uint) protocol;
#if NET20 || NET40
            this.cbPciLength = Marshal.SizeOf(typeof(SCARD_IO_REQUEST));
#else 
            this.cbPciLength = Marshal.SizeOf<SCARD_IO_REQUEST>();
#endif
        }

        internal SCARD_IO_REQUEST(CardProtocols protocol)
        {
            this.dwProtocol = (uint)protocol;
#if NET20 || NET40
            this.cbPciLength = Marshal.SizeOf(typeof(SCARD_IO_REQUEST));
#else 
            this.cbPciLength = Marshal.SizeOf<SCARD_IO_REQUEST>();
#endif
        }
    }
}
