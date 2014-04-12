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
    public class EidReader : IDisposable
    {
        private static readonly byte[] CMD_READ_BINARY = { 0x00, 0xB0, 0x00, 0x00, 0x00 };
        private static readonly byte[] EID_ATR_0 = new byte[] { 0x3B, 0x98, 0x13, 0x40, 0x0A, 0xA5, 0x03, 0x01, 0x01, 0x01, 0xAD, 0x13, 0x10 };
        private static readonly byte[] EID_ATR_1 = new byte[] { 0x3B, 0x98, 0x13, 0x40, 0x0A, 0xA5, 0x03, 0x01, 0x01, 0x01, 0xAD, 0x13, 0x11 };

        private CardSafeHandler handler;
        private CardContextSafeHandler context;

        public String Name { get; private set; }

        //private Thread deviceActionListenerThread;
        //private bool disposed;

        //public event EventHandler<DeviceEventArgs> ReaderAction;
        //public event EventHandler<DeviceEventArgs> CardAction;

        public EidReader(String name)
        {
            this.Name = name;

            //create a context
            uint retVal = NativeMethods.SCardEstablishContext(ContextScope.SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, out context);
            if (retVal != 0) throw new InvalidOperationException("Failed to create context for reader: " + retVal.ToString("X"));

            //Get current state
            SCARD_READERSTATE[] readerStates = new SCARD_READERSTATE[1];
            readerStates[0] = new SCARD_READERSTATE();
            readerStates[0].szReader = Name;
            readerStates[0].pvUserData = IntPtr.Zero;
            readerStates[0].dwCurrentState = ReaderState.SCARD_STATE_UNAWARE;
            retVal = NativeMethods.SCardGetStatusChange(context, 0, readerStates, 1);
            if (retVal == 0x80100009L) throw new ReaderException("The specified reader is unknown");
            if (retVal != 0) throw new InvalidOperationException("Failed to get status of reader: " + retVal.ToString("X"));


            //create a listener for this card
            //disposed = false;
            //deviceActionListenerThread = new Thread(DeviceActionListener);
            //deviceActionListenerThread.IsBackground = true;
            //deviceActionListenerThread.Start(readerStates);
        }

        ~EidReader()
        {
            Dispose(false);
        }

        
        /*
        private void DeviceActionListener(Object obj)
        {
            uint retVal;
            CardContextSafeHandler threadContext;
            
            retVal = NativeMethods.SCardEstablishContext(ContextScope.SCARD_SCOPE_SYSTEM, IntPtr.Zero, IntPtr.Zero, out threadContext);
            if (retVal != 0) throw new InvalidOperationException("Failed to create thread context for reader: " + retVal.ToString("X"));

            try
            {
                SCARD_READERSTATE[] readerStates = (SCARD_READERSTATE[]) obj;
                do
                {
                    readerStates[0].dwCurrentState = readerStates[0].dwEventState;
                    retVal = NativeMethods.SCardGetStatusChange(threadContext, 10000, readerStates, 1);
                    if (retVal == 0 //it isn't a timeout
                        && ((int)readerStates[0].dwCurrentState & 0x03FF) != ((int)readerStates[0].dwEventState & 0x03FF)) //filter out changes of unknown flags...
                    {
                        String[] cardNames = null;

                        int size = 0;
                        Char[] names = null;
                        retVal = NativeMethods.SCardListCards(threadContext, readerStates[0].rgbAtr, IntPtr.Zero, 0, names, ref size);
                        if (retVal == 0)
                        {
                            names = new Char[size];
                            retVal = NativeMethods.SCardListCards(threadContext, readerStates[0].rgbAtr, IntPtr.Zero, 0, names, ref size);
                            if (retVal == 0)
                            {
                                cardNames = MultiStringToStringArray(names);
                            }
                        }

                        OnCardAction(new DeviceEventArgs(cardNames == null ? null : cardNames[0],
                            ReaderStatusToCardStatus(readerStates[0].dwCurrentState),
                            ReaderStatusToCardStatus(readerStates[0].dwEventState)));

                        if ((readerStates[0].dwEventState & ReaderState.SCARD_STATE_UNKNOWN) == ReaderState.SCARD_STATE_UNKNOWN)
                        {
                            OnReaderAction(new DeviceEventArgs(readerName, DeviceState.Present, DeviceState.Missing));
                        }
                    }
                } while (!disposed && (retVal == 0 && (readerStates[0].dwEventState & ReaderState.SCARD_STATE_IGNORE) != ReaderState.SCARD_STATE_IGNORE) || retVal == 0x8010000A);
            }
            finally
            {
                threadContext.Close();
            }
        }
         

        private DeviceState ReaderStatusToCardStatus(ReaderState state)
        {
            if ((state & ReaderState.SCARD_STATE_EMPTY) == ReaderState.SCARD_STATE_EMPTY) return DeviceState.Missing;
            if ((state & (ReaderState.SCARD_STATE_PRESENT | ReaderState.SCARD_STATE_INUSE | ReaderState.SCARD_STATE_MUTE)) == ReaderState.SCARD_STATE_PRESENT) return DeviceState.Present;
            if ((state & ReaderState.SCARD_STATE_INUSE) == ReaderState.SCARD_STATE_INUSE) return DeviceState.InUse;
            if ((state & ReaderState.SCARD_STATE_MUTE) == ReaderState.SCARD_STATE_MUTE) return DeviceState.Mute;

            return DeviceState.Unknown;
        }

        protected void OnReaderAction(DeviceEventArgs e)
        {
            if (ReaderAction != null) ReaderAction(this, e);
        }

        protected void OnCardAction(DeviceEventArgs e)
        {
            if (CardAction != null) CardAction(this, e);
        }
         */

        private byte[] ReadRaw(byte[] fileSelect)
        {
            uint retVal;

            retVal = NativeMethods.SCardBeginTransaction(handler);
            if (retVal != 0) throw new InvalidOperationException("Failed to start transaction: " + retVal.ToString("X"));
            try
            {
                int rspLen = 258;
                byte[] rsp = new byte[rspLen];

                retVal = NativeMethods.SCardTransmit(handler, SCARD_IO_REQUEST.T0, fileSelect, fileSelect.Length, null, rsp, ref rspLen);
                if (retVal != 0) throw new InvalidOperationException("Failed to select file: " + retVal.ToString("X"));
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
                if (retVal != 0) throw new InvalidOperationException("Failed to end transaction: " + retVal.ToString("X"));
            }
        }

        public X509Certificate2 ReadCertificate(CertificateId cert)
        {
            byte[] fileSelect = ((FileSelectCmdAttribute)cert.GetType().GetMember(cert.ToString())[0].GetCustomAttributes(typeof(FileSelectCmdAttribute), false)[0]).Cmd;
            return new X509Certificate2(ReadRaw(fileSelect));
        }

        /// <summary>
        /// Connect with the card in the reader
        /// </summary>
        /// <exception cref="NoCardException">When no card is present</exception>
        /// <exception cref="UnsupportedCardException">When the inserted card isn't an eID</exception>
        /// <exception cref="ReaderException">When there is an issue with the reader</exception>
        /// <exception cref="InvalidOperationException">In case of an unknown native error</exception>
        public void Connect()
        {
            CardProtocols protocol;
            uint retVal = NativeMethods.SCardConnect(context, Name, CardShareMode.SCARD_SHARE_EXCLUSIVE, CardProtocols.SCARD_PROTOCOL_T0 | CardProtocols.SCARD_PROTOCOL_T1, out handler, out protocol);
            if (retVal == 0x80100069L) throw new NoCardException("Not card was found in the reader");
            if (retVal == 0x8010000BL) throw new ReaderException("The card is being accessed from a different context");
            if (retVal == 0x80100009L) throw new ReaderException("The specified reader is not connected");
            if (retVal != 0) throw new InvalidOperationException("Failed to open card reader: 0x" + retVal.ToString("X"));
            try
            {
                if (protocol != CardProtocols.SCARD_PROTOCOL_T0) throw new UnsupportedCardException("The library doesn't support this protocol of cards");

                int len = 0;
                retVal = NativeMethods.SCardGetAttrib(handler, CardAttrId.SCARD_ATTR_ATR_STRING, IntPtr.Zero, ref len);
                if (retVal != 0) throw new InvalidOperationException("Failed to get card type (length): " + retVal.ToString("X"));

                IntPtr atrIntPtr = Marshal.AllocHGlobal(len);
                try
                {
                    retVal = NativeMethods.SCardGetAttrib(handler, CardAttrId.SCARD_ATTR_ATR_STRING, atrIntPtr, ref len);
                    if (retVal != 0) throw new InvalidOperationException("Failed to get card type: " + retVal.ToString("X"));

                    byte[] atr = new byte[len];
                    Marshal.Copy(atrIntPtr, atr, 0, len);

                    if (!EID_ATR_1.SequenceEqual(atr) && !EID_ATR_0.SequenceEqual(atr)) throw new UnsupportedCardException("The card isn't a (supported version of the) belgium eID card");
                }
                finally
                {
                    Marshal.FreeHGlobal(atrIntPtr);
                }
            }
            catch
            {
                this.Disconnect();
            }
        }

        public void Disconnect()
        {
            handler.Close();
        }

        public bool IsConnected
        {
            get
            {
                return handler != null && !handler.IsClosed && !handler.IsInvalid;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool managed)
        {
            if (managed)
            {
                //disposed = true;
                if (IsConnected) this.Disconnect();
                context.Close();
            }
        }
    }
}
