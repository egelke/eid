
using System;
using Egelke.Eid.SmartCard.WinScard;

namespace Egelke.Eid.SmartCard
{
	public partial class SmartCardReader : IDisposable
	{
		private SafeCardContextHandle contextHandle;
		private SafeCardHandle cardHandle;
		private readonly ContextScope contextScope;

		private SmartCardReader(string name, ContextScope contextScope)
		{
			this.contextScope = contextScope;
			Name = name;
		}

		~SmartCardReader()
		{
			Dispose(false);
		}

		public string Name { get; private set; }

		/// <summary>
		/// Establishes a connection to the device.
		/// </summary>
		/// <returns>true indicates the connection was established. fase indicates the connection was already established earlier.</returns>
		public bool Connect()
		{
			if (Connected) return false;

			contextHandle = GetContext(contextScope);
			CardProtocols protocol;
			SmartCardException.CheckReturnCode(NativeMethods.SCardConnect(contextHandle, Name, CardShareMode.SCARD_SHARE_SHARED, CardProtocols.SCARD_PROTOCOL_T0, out cardHandle, out protocol));
			return true;
		}

		/// <summary>
		/// Closes the connection to the device.
		/// </summary>
		/// <returns>true indicates the connection has been closed. false indicates the connection was already closed.</returns>
		public void Disconnect()
		{
			if (!Connected) return;

			Dispose(true);
		}

		public bool Connected { get { return contextHandle != null; } }

		public CardState GetCardState()
		{
			EnsureConnected();
			
			var readerLenght = 0;
			CardState state;
			CardProtocols prot;
			var atr = new byte[32];
			var length = 32;

			SmartCardException.CheckReturnCode(NativeMethods.SCardStatus(cardHandle, null, ref readerLenght, out state, out prot, atr, ref length));

			return state;
		}

		public override string ToString()
		{
			return Name;
		}

		public void Dispose()
		{
			Dispose(true);
		}

		// Just in case, this is note really necessary
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;
			
			cardHandle.Dispose();
			cardHandle = null;

			contextHandle.Dispose();
			contextHandle = null;
		}

		private void EnsureConnected()
		{
			if(!Connected) throw new InvalidOperationException("This operation requies the reader to be connected. Call Conntect() to establish a connection.");
		}
	}
}