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

namespace Egelke.Eid.SmartCard.WinScard
{
    internal enum ProtocolControlInformation : uint
    {
		/// <summary>
		/// An asynchronous, character-oriented half-duplex transmission protocol.
		/// </summary>
        SCARD_PCI_T0 = 1,
		/// <summary>
		/// An asynchronous, block-oriented half-duplex transmission protocol.
		/// </summary>
        SCARD_PCI_T1 = 2,
        SCARD_PCI_RAW = 4 //not supported, but defined...
    }
}
