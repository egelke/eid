﻿/*
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
using System.Runtime.InteropServices;
using System.Text;

namespace Egelke.Eid.Client
{
    internal class CardContextSafeHandler : SafeHandle
    {
        private CardContextSafeHandler()
            : base(IntPtr.Zero, true)
        {
        }

        public CardContextSafeHandler(IntPtr preexistingHandle)
            : base(IntPtr.Zero, true)
        {
            this.handle = preexistingHandle;
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.SCardReleaseContext(handle) == 0;
        }

        public override bool IsInvalid => handle == IntPtr.Zero || handle == new IntPtr(-1);
    }
}
