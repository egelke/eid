using System.Linq;
using Egelke.Eid.SmartCard;
using NUnit.Framework;

namespace Egelke.Eid.Client.Test.WithCard
{
	[TestFixture]
	public class When_enumerating_available_readers
	{
		[Test]
		public void at_least_one_reader_should_be_listed()
		{
			var readers = SmartCardReader.GetReaders();
			Assert.Greater(0, readers.Count());
		}
	}
}
