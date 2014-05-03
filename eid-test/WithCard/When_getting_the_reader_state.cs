using Egelke.Eid.SmartCard;
using Egelke.Eid.SmartCard.WinScard;
using NUnit.Framework;

namespace Egelke.Eid.Client.Test.WithCard
{
	[TestFixture]
	public class When_getting_the_reader_state
	{
		[Test]
		public void the_reader_satatus_should_be_specific()
		{
			using (var reader = SmartCardReader.GetReader(TestContext.READER_NAME))
			{
				reader.Connect();
				var cardState = reader.GetCardState();
				
				Assert.AreEqual(CardState.Specific, cardState);
			}
		}
	}
}