using Egelke.Eid.SmartCard;
using NUnit.Framework;

namespace Egelke.Eid.Client.Test.WithCard
{
	[TestFixture]
	public class When_connecting_to_an_non_exclusively_connected_reader
	{
		[Test]
		public void exclusive_mode_should_succeed()
		{
			using (var reader1 = SmartCardReader.GetReader(TestContext.READER_NAME))
			{
				reader1.Connect(true);

				using (var reader2 = SmartCardReader.GetReader(TestContext.READER_NAME))
				{
					reader2.Connect(true);
					Assert.IsTrue(reader2.Connected);
				}
			}
		}

		[Test]
		public void shared_mode_should_succeed()
		{
			using (var reader1 = SmartCardReader.GetReader(TestContext.READER_NAME))
			{
				reader1.Connect(true);

				using (var reader2 = SmartCardReader.GetReader(TestContext.READER_NAME))
				{
					reader2.Connect(true);
					Assert.IsTrue(reader2.Connected);
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