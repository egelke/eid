using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Egelke.Eid.SmartCard.WinScard;

namespace Egelke.Eid.SmartCard
{
	public partial class SmartCardReader
	{
		public static IEnumerable<SmartCardReader> GetReaders()
		{
			using (var context = GetContext())
			{
				var bufferSize = 0;
				char[] buffer = null;
				var returnCode = NativeMethods.SCardListReaders(context, null, buffer, ref bufferSize);

				if (returnCode == (int)SmartCardErrors.SCARD_E_NO_READERS_AVAILABLE) return Enumerable.Empty<SmartCardReader>();

				SmartCardException.CheckReturnCode(returnCode);

				return ParseNames(buffer).Select(name => new SmartCardReader(name));
			}
		}

		public static SmartCardReader GetReader(string readerName)
		{
			throw new NotImplementedException();
		}

		public static SmartCardReader GetReaderWithCard(string cardName)
		{
			throw new NotImplementedException();
		}

		private static SafeCardContextHandle GetContext()
		{
			SafeCardContextHandle context;
			SmartCardException.CheckReturnCode(NativeMethods.SCardEstablishContext(ContextScope.SCARD_SCOPE_SYSTEM, IntPtr.Zero, IntPtr.Zero, out context));
			return context;
		}

		private static IEnumerable<String> ParseNames(char[] multiStr)
		{
			if (multiStr.Length <= 1) yield break;
			
			var i = 0;
			var str = new StringBuilder();

			while (true)
			{
				var value = multiStr[i++];
				
				if (value != '\0')
				{
					str.Append(value);
				}
				else
				{
					yield return str.ToString();
					str.Clear();
					if (multiStr[i] == '\0') break;
				}
			}
		}
	}
}
