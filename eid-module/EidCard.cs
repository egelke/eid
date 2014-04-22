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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace Egelke.Eid.Client
{
    public class EidCard : IDisposable
    {
        private static readonly byte[] CMD_READ_BINARY = { 0x00, 0xB0, 0x00, 0x00, 0x00 };

        private readonly CardSafeHandler handler;
        private CardContextSafeHandler context;

        public String ReaderName { get; private set; }

        internal EidCard(CardContextSafeHandler context, String readerName)
        {
            this.context = context;
            this.ReaderName = readerName;

            CardProtocols protocol;
            uint retVal = NativeMethods.SCardConnect(context, ReaderName, CardShareMode.SCARD_SHARE_SHARED, CardProtocols.SCARD_PROTOCOL_T0 | CardProtocols.SCARD_PROTOCOL_T1, out handler, out protocol);
            if (retVal == 0x80100069L) throw new NoCardException("Not card was found in the reader");
            if (retVal == 0x8010000BL) throw new ReaderException("The card is being accessed from a different context");
            if (retVal == 0x80100009L) throw new ReaderException("The specified reader does not exist");
            if (retVal != 0) throw new InvalidOperationException("Failed to open card reader: 0x" + retVal.ToString("X"));
        }

        ~EidCard()
        {
            Dispose(false);
        }

        private byte[] ReadRaw(byte[] fileSelect)
        {
            uint retVal;

            retVal = NativeMethods.SCardBeginTransaction(handler);
            if (retVal == 0x80100017) throw new ReaderException("The card reader isn't available any more");
            if (retVal == 0x80100069) throw new NoCardException("The card has been removed");
            if (retVal != 0) throw new InvalidOperationException("Failed to start transaction: 0x" + retVal.ToString("X"));
            try
            {
                int rspLen = 258;
                byte[] rsp = new byte[rspLen];

                retVal = NativeMethods.SCardTransmit(handler, SCARD_IO_REQUEST.T0, fileSelect, fileSelect.Length, null, rsp, ref rspLen);
                if (retVal == 0x80100017) throw new ReaderException("The card reader isn't available any more");
                if (retVal == 0x80100069) throw new NoCardException("The card has been removed");
                if (retVal != 0) throw new InvalidOperationException("Failed to select file: 0x" + retVal.ToString("X"));
                if (rspLen < 0)
                {
                    throw new InvalidOperationException("Failed to select file SW1=? SW2=?");
                }
                if (rsp[0] != 0x90 || rsp[1] != 0x0)
                {
                    throw new InvalidOperationException("Failed to select file SW1=" + rsp[0].ToString("X") + " SW2=" + rsp[1].ToString("X"));
                }

                int offset = 0;
                byte[] cmd = new byte[CMD_READ_BINARY.Length];
                Array.Copy(CMD_READ_BINARY, cmd, cmd.Length);
                MemoryStream bytes = new MemoryStream();
                do
                {
                    cmd[2] = (byte)(offset >> 8);
                    cmd[3] = (byte)(offset & 0xFF);

                    rspLen = rsp.Length;
                    retVal = NativeMethods.SCardTransmit(handler, SCARD_IO_REQUEST.T0, cmd, cmd.Length, null, rsp, ref rspLen);
                    if (retVal == 0x80100017) throw new ReaderException("The card reader isn't available any more");
                    if (retVal == 0x80100069) throw new NoCardException("The card has been removed");
                    if (retVal != 0) throw new InvalidOperationException("Failed to read bytes: " + retVal.ToString("X"));
                    if (rspLen < 0)
                    {
                        throw new InvalidOperationException("Failed to read bytes SW1=? SW2=?");
                    }
                    if (rsp[rspLen - 2] == 0x6B && rsp[rspLen - 1] == 0x0)
                    {
                        break; // Finished, there where no more bytes
                    }
                    if (rsp[rspLen - 2] == 0x6C)
                    {
                        cmd[4] = rsp[1];// Almost finished, reading less
                        continue;
                    }
                    if (rsp[rspLen - 2] != 0x90 || rsp[rspLen - 1] != 0x0)
                    {
                        throw new InvalidOperationException("Failed to read bytes SW1=" + rsp[0].ToString("X") + " SW2=" + rsp[1].ToString("X"));
                    }

                    bytes.Write(rsp, 0, rspLen - 2);
                    offset += rspLen - 2;
                } while (rspLen == 258 || rsp[0] == 0x6C);

                return bytes.ToArray();
            }
            finally
            {
                retVal = NativeMethods.SCardEndTransaction(handler, CardDisposition.SCARD_LEAVE_CARD);
                if (retVal == 0x80100017) throw new ReaderException("The card reader isn't available any more");
                if (retVal == 0x80100069) throw new NoCardException("The card has been removed");
                if (retVal != 0) throw new InvalidOperationException("Failed to end transaction: 0x" + retVal.ToString("X"));
            }
        }

        public X509Certificate2 ReadCertificate(CertificateId cert)
        {
            byte[] fileSelect = ((FileSelectCmdAttribute)cert.GetType().GetMember(cert.ToString())[0].GetCustomAttributes(typeof(FileSelectCmdAttribute), false)[0]).Cmd;
            return new X509Certificate2(ReadRaw(fileSelect));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool managed)
        {
            if (managed)
            {
                if (!handler.IsInvalid && !handler.IsClosed) handler.Close();
            }
        }
    }
}
