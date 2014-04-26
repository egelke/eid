
using System;
using Egelke.Eid.SmartCard.WinScard;

namespace Egelke.Eid.SmartCard
{
	public partial class SmartCardReader : IDisposable
	{
		private SafeCardContextHandle contextHandle;
		private SafeCardHandler cardHandle;

		private SmartCardReader(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }

		public void Connect()
		{
			if (Connected) return;

			contextHandle = GetContext();
			CardProtocols protocol;
			NativeMethods.SCardConnect(contextHandle, Name, CardShareMode.SCARD_SHARE_SHARED, CardProtocols.SCARD_PROTOCOL_T0, out cardHandle, out protocol);
		}

		public bool Connected { get { return contextHandle != null; } }

		public CardState GetStatus()
		{
			Connect();
			int readerLenght = 0;
			CardState state;
			CardProtocols prot;
			byte[] atr = new byte[32];
			var length = 32;
			//http://ludovic.rousseau.free.fr/softwares/pcsc-tools/smartcard_list.txt
			var ret = NativeMethods.SCardStatus(cardHandle, null, ref readerLenght, out state, out prot, atr, ref length);

			return state;
		}

		public override string ToString()
		{
			return Name;
		}

		public void Dispose()
		{
			// TODO
		}
	}
}