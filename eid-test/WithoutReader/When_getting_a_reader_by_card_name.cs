using System;
using System.Diagnostics;
using Egelke.Eid.SmartCard;
using NUnit.Framework;

namespace Egelke.Eid.Client.Test.WithoutReader
{
	[TestFixture]
	public class When_getting_a_reader_by_card_name
	{
		[Test]
		public void a_null_ref_should_be_returned()
		{
			var reader = SmartCardReader.GetReaderWithCard(AtrName.BelgianEid);

			Assert.IsNull(reader);
		}

		[Test]
		public void the_timeout_should_be_elapsed()
		{
			var sw = Stopwatch.StartNew();
			var timeout = TimeSpan.FromMilliseconds(200);

			var reader = SmartCardReader.GetReaderWithCard(AtrName.BelgianEid, timeout);
			sw.Stop();

			Assert.GreaterOrEqual(sw.Elapsed, timeout);
		}
	}
}