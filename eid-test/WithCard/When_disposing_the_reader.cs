using Egelke.Eid.SmartCard;
using NUnit.Framework;

namespace Egelke.Eid.Client.Test.WithCard
{
	[TestFixture]
	public class When_disposing_the_reader
	{
		[Test]
		public void the_card_should_become_disconnected()
		{
			var reader = SmartCardReader.GetReader(TestContext.READER_NAME);

			reader.Connect();
			reader.Dispose();

			Assert.IsFalse(reader.Connected);
		}
	}
}