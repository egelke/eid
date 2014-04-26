using System.Linq;
using Egelke.Eid.SmartCard;
using NUnit.Framework;

namespace Egelke.Eid.Client.Test.WithoutReader
{
	[TestFixture]
	public class When_enumerating_available_readers
	{
		[Test]
		public void no_reader_should_be_returned()
		{
			var readers = SmartCardReader.GetReaders();
			Assert.AreEqual(0, readers.Count());
		}
	}
}
