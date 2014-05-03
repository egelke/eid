using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Egelke.Eid.SmartCard
{
	/// <summary>
	/// Reprsents a list of ATR expressions that match specific cards. Source: http://ludovic.rousseau.free.fr/softwares/pcsc-tools/smartcard_list.txt
	/// </summary>
	public class AtrName
	{
		public static AtrName BelgianEid = new AtrName("Belgian eID Card", new List<byte[]>
		{
			new byte[] { 0x3B, 0x98, 0x13, 0x40, 0x0A, 0xA5, 0x03, 0x01, 0x01, 0x01, 0xAD, 0x13, 0x11 },
			new byte[] { 0x3B, 0x98, 0x94, 0x40, 0x0A, 0xA5, 0x03, 0x01, 0x01, 0x01, 0xAD, 0x13, 0x10 }
		});
		
		private IList<byte[]> masks;

		public AtrName(string cardName, IEnumerable<byte[]> masks)
		{
			this.masks = new List<byte[]>(masks);
			CardName = cardName;
		}

		public string CardName { get; private set; }

		public IEnumerable<byte[]> Mask { get { return masks; } }

		public bool Matches(byte[] value, int length)
		{
			return masks.Any(m => Matches(m, value, length));
		}

		private static bool Matches(byte[] mask, byte[] value, int length)
		{
			if (value == null) throw new ArgumentNullException("value");
			if (length > value.Length) throw new ArgumentException("length must be smaller than buffer size.", "length");

			if (length !=mask.Length) return false;

			for (var i = 0; i < length; i++)
			{
				if ((value[i] & ~mask[i]) != 0) return false;
			}
			return true;
		}
	}
}