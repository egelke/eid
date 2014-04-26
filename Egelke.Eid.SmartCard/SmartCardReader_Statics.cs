using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Egelke.Eid.SmartCard.WinScard;

namespace Egelke.Eid.SmartCard
{
	public partial class SmartCardReader
	{
		/// <summary>
		/// Enumerates all smart card readers on the current system.
		/// </summary>
		public static IEnumerable<SmartCardReader> GetReaders()
		{
			using (var context = GetContext())
				return GetReaders(context);
		}



		/// <summary>
		/// Gets the reader with the specified name or null if the reader is not available.
		/// </summary>
		public static SmartCardReader GetReader(string readerName)
		{
			return GetReaders().FirstOrDefault(r => r.Name.Equals(readerName, StringComparison.CurrentCultureIgnoreCase));
			
		}

		/// <summary>
		/// Returns the smart card reader that has the card with the specified name inserted or null if the card is not found.
		/// </summary>
		/// <param name="cardName">The ATR name of the card to search.</param>
		public static SmartCardReader GetReaderWithCard(string atrName)
		{
			throw new NotImplementedException();
		}

		private static SafeCardContextHandle GetContext()
		{
			SafeCardContextHandle context;
			SmartCardException.CheckReturnCode(NativeMethods.SCardEstablishContext(ContextScope.SCARD_SCOPE_SYSTEM, IntPtr.Zero, IntPtr.Zero, out context));
			return context;
		}

		private static IEnumerable<SmartCardReader> GetReaders(SafeCardContextHandle context)
		{
			var bufferSize = 0;

			// get buffer size
			var returnCode = NativeMethods.SCardListReaders(context, null, null, ref bufferSize);
			if (returnCode == (int)SmartCardErrors.SCARD_E_NO_READERS_AVAILABLE) return Enumerable.Empty<SmartCardReader>();
			SmartCardException.CheckReturnCode(returnCode);

			var buffer = new char[bufferSize];
			returnCode = NativeMethods.SCardListReaders(context, null, buffer, ref bufferSize);
			if (returnCode == (int)SmartCardErrors.SCARD_E_NO_READERS_AVAILABLE) return Enumerable.Empty<SmartCardReader>();
			SmartCardException.CheckReturnCode(returnCode);

			return ParseNames(buffer).Select(name => new SmartCardReader(name));
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
