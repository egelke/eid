
using System;
using System.Diagnostics;
using Egelke.Eid.SmartCard;
using NUnit.Framework;

namespace Egelke.Eid.Client.Test.WithCard
{
	[TestFixture]
	public class When_getting_a_reader_by_card_name
	{
		[Test]
		public void the_card_should_be_returned()
		{
			var reader = SmartCardReader.GetReaderWithCard(AtrName.BelgianEid);

			Assert.IsNotNull(reader);
		}

		[Test]
		public void the_timeout_should_not_be_elapsed()
		{
			var sw = Stopwatch.StartNew();
			var timeout = TimeSpan.FromSeconds(1);

			var reader = SmartCardReader.GetReaderWithCard(AtrName.BelgianEid, timeout);
			sw.Stop();

			Assert.IsNotNull(reader);
			Assert.Less(sw.Elapsed, timeout);
		}
	}
}