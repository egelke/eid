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

namespace Egelke.Eid.Client.Model
{
    public class Address
    {
        public Address(byte[] file)
        {
            IDictionary<byte, byte[]> d = file.Parse();

            StreetAndNumber = d[0x01].ToStr();
            Zip = d[0x02].ToStr();
            Municipality = d[0x03].ToStr();
        }

        public String StreetAndNumber { get; }

        public String Zip { get; }

        public String Municipality { get; }


    }
}
