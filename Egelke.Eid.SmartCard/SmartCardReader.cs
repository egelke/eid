
using System;
using Egelke.Eid.SmartCard.WinScard;

namespace Egelke.Eid.SmartCard
{
	public partial class SmartCardReader : IDisposable
	{
		private SafeCardContextHandle contextHandle;
		private SafeCardHandle cardHandle;

		private SmartCardReader(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }

		/// <summary>
		/// Establishes a connection to the device.
		/// </summary>
		/// <returns>true indicates the connection was established. fase indicates the connection was already established earlier.</returns>
		public bool Connect()
		{
			if (Connected) return false;

			contextHandle = GetContext();
			CardProtocols protocol;
			SmartCardException.CheckReturnCode(NativeMethods.SCardConnect(contextHandle, Name, CardShareMode.SCARD_SHARE_SHARED, CardProtocols.SCARD_PROTOCOL_T0, out cardHandle, out protocol));
			return true;
		}

		/// <summary>
		/// Closes the connection to the device.
		/// </summary>
		/// <returns>true indicates the connection has been closed. false indicates the connection was already closed.</returns>
		public bool Disconnect()
		{
			if (!Connected) return false;

			cardHandle.Dispose();
			cardHandle = null;

			contextHandle.Dispose();
			contextHandle = null;

			return true;
		}

		public bool Connected { get { return contextHandle != null; } }

		public CardState GetCardState()
		{
			return EnsureConntected(() =>
			{
				var readerLenght = 0;
				CardState state;
				CardProtocols prot;
				var atr = new byte[32];
				var length = 32;

				SmartCardException.CheckReturnCode(NativeMethods.SCardStatus(cardHandle, null, ref readerLenght, out state, out prot, atr, ref length));

				return state;
			});
		}

		public override string ToString()
		{
			return Name;
		}

		public void Dispose()
		{
			// TODO
		}

		private T EnsureConntected<T>(Func<T> func)
		{
			var shouldDisconnect = Connect();
			var ret = func();
			if (shouldDisconnect) Disconnect();

			return ret;
		}

		private void EnsureConntected(Action action)
		{
			var shouldDisconnect = Connect();
			action();
			if (shouldDisconnect) Disconnect();
		}
	}
}