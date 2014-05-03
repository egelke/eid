using Egelke.Eid.SmartCard;
using NUnit.Framework;

namespace Egelke.Eid.Client.Test.WithReaderButNoCard
{
	[TestFixture]
	public class When_connecting_to_the_reader
	{
		[Test]
		public void a_smart_card_exception_should_be_thrown()
		{
			SmartCardException exception = null;

			var reader = SmartCardReader.GetReader(TestContext.READER_NAME);
			try
			{
				reader.Connect();
			}
			catch (SmartCardException ex)
			{
				exception = ex;
			}

			Assert.NotNull(exception);
			Assert.AreEqual(-2146434967, exception.NativeErrorCode);
		}
	}
}