using System.Linq;
using Egelke.Eid.SmartCard;
using Egelke.Eid.SmartCard.WinScard;
using NUnit.Framework;

namespace Egelke.Eid.Client.Test.WithReaderButNoCard
{
	[TestFixture]
	public class When_enumerating_available_readers_in_system_scope
	{
		[Test]
		public void at_least_one_reader_should_be_listed()
		{
			var readers = SmartCardReader.GetReaders(ContextScope.System);
			Assert.Greater(readers.Count(), 0);
		}
	}
}
