using System;

namespace Egelke.Eid.SmartCard
{
	internal class Transaction : IDisposable
	{
		private readonly SmartCardReader smartCardReader;

		public Transaction(SmartCardReader smartCardReader)
		{
			this.smartCardReader = smartCardReader;
		}

		public void Dispose()
		{
			smartCardReader.EndTransaction();
		}
	}
}