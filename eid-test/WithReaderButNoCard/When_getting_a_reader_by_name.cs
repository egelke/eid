using Egelke.Eid.SmartCard;
using NUnit.Framework;

namespace Egelke.Eid.Client.Test.WithReaderButNoCard
{
	[TestFixture]
	public class When_getting_a_reader_by_name
	{
		[Test]
		public void the_reader_should_be_returned()
		{
			var reader = SmartCardReader.GetReader(TestContext.READER_NAME);
			reader.GetCardState();
			Assert.IsNotNull(reader);
		}
	}
}