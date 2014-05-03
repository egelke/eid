using Egelke.Eid.SmartCard;
using NUnit.Core;
using NUnit.Framework;

namespace Egelke.Eid.Client.Test.WithCard
{
	[TestFixture]
	public class When_connecting_to_an_exclusively_connected_reader
	{
		[Test]
		public void exclusive_mode_should_fail()
		{
			using (var reader1 = SmartCardReader.GetReader(TestContext.READER_NAME))
			{
				reader1.Connect();

				using (var reader2 = SmartCardReader.GetReader(TestContext.READER_NAME))
				{
					AssertExceptionOnConnect(reader2, false);
				}
			}

		}

		[Test]
		public void shared_mode_should_fail()
		{
			using (var reader1 = SmartCardReader.GetReader(TestContext.READER_NAME))
			{
				reader1.Connect();

				using (var reader2 = SmartCardReader.GetReader(TestContext.READER_NAME))
				{
					AssertExceptionOnConnect(reader2, true);
				}
			}

		}

		private void AssertExceptionOnConnect(SmartCardReader reader, bool shared)
		{
			SmartCardException exception = null;
			try
			{
				reader.Connect(true);
			}
			catch (SmartCardException ex)
			{
				exception = ex;
			}

			Assert.NotNull(exception);
			Assert.AreEqual(-2146435061, exception.NativeErrorCode);
		}
	}
}