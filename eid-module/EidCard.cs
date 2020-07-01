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
using Egelke.Eid.Client.Model;

namespace Egelke.Eid.Client
{
    public class EidCard : Card
    {
        private static readonly byte[] ATR_VAL = { 0x3b, 0x98, 0x00, 0x40, 0x00, 0xa5, 0x03, 0x01, 0x01, 0x01, 0xad, 0x13, 0x00 };
        private static readonly byte[] ATR_MASK = { 0xff, 0xff, 0x00, 0xff, 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x00 };
        private static readonly byte[] ATR18_VAL = { 0x3b, 0x7f, 0x96, 0x00, 0x00, 0x80, 0x31, 0x80, 0x65, 0xb0, 0x85, 0x04, 0x01, 0x20, 0x12, 0x0f, 0xff, 0x82, 0x90, 0x00 };
        private static readonly byte[] ATR18_MASK = { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

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

        public static bool IsEid(byte[] atr)
        {
            if (atr.Length == ATR_VAL.Length) {
                int i = 0;
                while (i < atr.Length && (atr[i] & ATR_MASK[i]) == ATR_VAL[i]) { i++; }
                if (i == atr.Length) return true;
            }
            
            if (atr.Length == ATR18_VAL.Length)
            {
                int i = 0;
                while (i < atr.Length && (atr[i] & ATR18_MASK[i]) == ATR18_VAL[i]) { i++; }
                if (i == atr.Length) return true;
            }

            return false;
        }

        internal EidCard(CardContextSafeHandler context, String readerName, byte[] atr) 
            : base(context, readerName, atr)
        {
            
        }

        public byte[] ReadRaw(EidFile file) => ReadBinary(fileSelectors[file]);

        public X509Certificate2 AuthCert => new X509Certificate2(ReadRaw(EidFile.AuthCert));

        public X509Certificate2 SignCert => new X509Certificate2(ReadRaw(EidFile.SignCert));

        public X509Certificate2 CaCert => new X509Certificate2(ReadRaw(EidFile.CaCert));

        public X509Certificate2 RootCert => new X509Certificate2(ReadRaw(EidFile.RootCert));

        public X509Certificate2 RrnCert => new X509Certificate2(ReadRaw(EidFile.RrnCert));

        public byte[] Picture => ReadRaw(EidFile.Picture);

        public Address Address => new Address(ReadRaw(EidFile.Address));

        public Identity Identity => new Identity(ReadRaw(EidFile.Id));

    }
}
