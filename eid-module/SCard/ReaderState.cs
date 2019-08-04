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

namespace Egelke.Eid.Client
{
    [Flags]
    internal enum ReaderState : int
    {
        SCARD_STATE_UNAWARE     = 0x0000,
        SCARD_STATE_IGNORE      = 0x0001,
        SCARD_STATE_CHANGED     = 0x0002,
        SCARD_STATE_UNKNOWN     = 0x0004,
        SCARD_STATE_UNAVAILABLE = 0x0008,
        SCARD_STATE_EMPTY       = 0x0010,
        SCARD_STATE_PRESENT     = 0x0020,
        SCARD_STATE_ATRMATCH    = 0x0040,
        SCARD_STATE_EXCLUSIVE   = 0x0080,
        SCARD_STATE_INUSE       = 0x0100,
        SCARD_STATE_MUTE        = 0x0200
    }
}
