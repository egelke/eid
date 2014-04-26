using System.Linq;
using System.Text.RegularExpressions;

namespace Egelke.Eid.SmartCard
{
	/// <summary>
	/// Reprsents a list of ATR expressions that match specific cards. Source: http://ludovic.rousseau.free.fr/softwares/pcsc-tools/smartcard_list.txt
	/// </summary>
	public class AtrName
	{
		public static AtrName BelgianEid = new AtrName("Belgian eID Card", "3B9894400AA503010101AD131[0|1]");

		public AtrName(string cardName, string expression)
		{
			CardName = cardName;
			Expression = new Regex(expression, RegexOptions.Compiled);
		}

		public string CardName { get; private set; }

		public Regex Expression { get; private set; }

		public bool Matches(byte[] value)
		{
			var s = new string(value.SelectMany(b => b.ToString("X").ToCharArray()).ToArray());
			return Expression.IsMatch(s);
		}
	}
}