using Egelke.Eid.SmartCard;
using NUnit.Framework;

namespace Egelke.Eid.Client.Test.WithoutReader
{
	[TestFixture]
	public class When_getting_a_reader_by_name
	{
		[Test]
		public void a_null_reference_should_be_returned()
		{
			var reader = SmartCardReader.GetReader(TestContext.READER_NAME);
			Assert.IsNull(reader);
		}
	}
}