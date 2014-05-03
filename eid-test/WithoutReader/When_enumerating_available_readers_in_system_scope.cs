using System.Linq;
using Egelke.Eid.SmartCard;
using Egelke.Eid.SmartCard.WinScard;
using NUnit.Framework;

namespace Egelke.Eid.Client.Test.WithoutReader
{
	[TestFixture]
	public class When_enumerating_available_readers_in_system_scope
	{
		[Test]
		public void no_reader_should_be_listed()
		{
			var readers = SmartCardReader.GetReaders(ContextScope.System);
			Assert.AreEqual(0, readers.Count());
		}
	}
}
