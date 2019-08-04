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
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Egelke.Eid.Client
{
    public class EidCard : Card
    {
        public static readonly string[] KNOWN_NAMES = { "Beid" };

        private static readonly Dictionary<EidFile, byte[]> fileSelectors = new Dictionary<EidFile, byte[]>();

        static EidCard() {
            fileSelectors.Add(EidFile.AuthCert, new byte[] { 0x3F, 0x00, 0xDF, 0x00, 0x50, 0x38});
            fileSelectors.Add(EidFile.SignCert, new byte[] { 0x3F, 0x00, 0xDF, 0x00, 0x50, 0x39 });
            fileSelectors.Add(EidFile.CaCert, new byte[] { 0x3F, 0x00, 0xDF, 0x00, 0x50, 0x3A });
            fileSelectors.Add(EidFile.RootCert, new byte[] { 0x3F, 0x00, 0xDF, 0x00, 0x50, 0x3B });
            fileSelectors.Add(EidFile.RrnCert, new byte[] { 0x3F, 0x00, 0xDF, 0x00, 0x50, 0x3C });

            fileSelectors.Add(EidFile.Id, new byte[] { 0x3F, 0x00, 0xDF, 0x01, 0x40, 0x31 });
            fileSelectors.Add(EidFile.IdSig, new byte[] { 0x3F, 0x00, 0xDF, 0x01, 0x40, 0x32 });
            fileSelectors.Add(EidFile.Address, new byte[] { 0x3F, 0x00, 0xDF, 0x01, 0x40, 0x33 });
            fileSelectors.Add(EidFile.AddressSig, new byte[] { 0x3F, 0x00, 0xDF, 0x01, 0x40, 0x34 });
            fileSelectors.Add(EidFile.Picture, new byte[] { 0x3F, 0x00, 0xDF, 0x01, 0x40, 0x35 });
        }

        public static bool IsEid(List<String> names)
        {
            foreach (String name in KNOWN_NAMES)
            {
                if (names.Contains(name)) return true;
            }
            return false;
        }

        internal EidCard(CardContextSafeHandler context, String readerName, byte[] atr) 
            : base(context, readerName, atr)
        {
            
        }

        public byte[] ReadRaw(EidFile file)
        {
            return ReadBinary(fileSelectors[file]);
        }

        public X509Certificate2 AuthCert
        {
            get
            {
                return new X509Certificate2(ReadRaw(EidFile.AuthCert));
            }
        }

        public X509Certificate2 SignCert
        {
            get
            {
                return new X509Certificate2(ReadRaw(EidFile.SignCert));
            }
        }

        public X509Certificate2 CaCert
        {
            get
            {
                return new X509Certificate2(ReadRaw(EidFile.CaCert));
            }
        }

        public X509Certificate2 RootCert
        {
            get
            {
                return new X509Certificate2(ReadRaw(EidFile.RootCert));
            }
        }

        public X509Certificate2 RrnCert
        {
            get
            {
                return new X509Certificate2(ReadRaw(EidFile.RrnCert));
            }
        }

        public byte[] Picture
        {
            get
            {
                return ReadRaw(EidFile.Picture);
            }
        }

       
    }
}
