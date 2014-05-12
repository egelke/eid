using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		public static IEnumerable<SmartCardReader> GetReaders(ContextScope scope = ContextScope.User)
		{
			using (var context = GetContext(scope))
				return GetReaders(context, scope).ToArray(); // avoid multiple enumerations in client code
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
		public static SmartCardReader GetReaderWithCard(AtrName atrName, TimeSpan timeout, ContextScope scope = ContextScope.User)
		{
			var stopwatch = Stopwatch.StartNew();

			using (var context = GetContext(scope))
			{
				var readerStates = GetReaderNames(context, scope).Select(name => new SCARD_READERSTATE { szReader = name }).ToArray();

				do
				{
					var remainingTimeout = timeout - stopwatch.Elapsed;

					GetReaderStateChanges(context, readerStates, remainingTimeout < TimeSpan.Zero ? TimeSpan.Zero : remainingTimeout);

					if (readerStates.Any(s => atrName.Matches(s.rgbAtr, s.cbAtr)))
						return new SmartCardReader(readerStates.FirstOrDefault(s => atrName.Matches(s.rgbAtr, s.cbAtr)).szReader, scope);

				} while (stopwatch.Elapsed < timeout);

				return null;
			}
		}

		private static void GetReaderStateChanges(SafeCardContextHandle context, SCARD_READERSTATE[] readerStates, TimeSpan timeout)
		{
			// this is required to make the timeout work (e.g. because there are changes)
			// the first time this method is called, the native method will return immediately
			for (var i = 0; i < readerStates.Length; i++)
			{
				readerStates[i].dwCurrentState = readerStates[i].dwEventState;
			}

			var ret = NativeMethods.SCardGetStatusChange(context, (int) timeout.TotalMilliseconds, readerStates, readerStates.Length);
			if (ret == (int) SmartCardErrors.SCARD_E_TIMEOUT) return;

			SmartCardException.CheckReturnCode(ret);
		}

		/// <summary>
		/// Returns the smart card reader that has the card with the specified name inserted or null if the card is not found.
		/// </summary>
		public static SmartCardReader GetReaderWithCard(AtrName atrName, ContextScope scope = ContextScope.User)
		{
			return GetReaderWithCard(atrName, TimeSpan.Zero, scope);
		}

		private static SafeCardContextHandle GetContext(ContextScope scope)
		{
			SafeCardContextHandle context;
			SmartCardException.CheckReturnCode(NativeMethods.SCardEstablishContext(scope, IntPtr.Zero, IntPtr.Zero, out context));
			return context;
		}

		private static IEnumerable<string> GetReaderNames(SafeCardContextHandle context, ContextScope scope)
		{
			var bufferSize = 0;

			// get buffer size
			var returnCode = NativeMethods.SCardListReaders(context, null, null, ref bufferSize);
			if (returnCode == (int)SmartCardErrors.SCARD_E_NO_READERS_AVAILABLE) return Enumerable.Empty<string>();
			SmartCardException.CheckReturnCode(returnCode);

			var buffer = new char[bufferSize];
			returnCode = NativeMethods.SCardListReaders(context, null, buffer, ref bufferSize);
			if (returnCode == (int)SmartCardErrors.SCARD_E_NO_READERS_AVAILABLE) return Enumerable.Empty<string>();
			SmartCardException.CheckReturnCode(returnCode);

			return ParseNames(buffer);
		}

		private static IEnumerable<SmartCardReader> GetReaders(SafeCardContextHandle context, ContextScope scope)
		{
			return GetReaderNames(context, scope).Select(name => new SmartCardReader(name, scope));
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
