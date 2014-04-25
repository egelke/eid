
using System.ComponentModel;

namespace Egelke.Eid.SmartCard
{
	/// <summary>
	/// Represents an error that occurs while interacting with a smart card reader.
	/// </summary>
	public class SmartCardException : Win32Exception
	{
		public SmartCardException(int error) : base(error) { }

		public SmartCardException(string message) : base(message) { }

		/// <summary>
		/// Throws a SmartCardException if the value represents a bad return value.
		/// </summary>
		/// <param name="value">The WINSCARD API return value to check.</param>
		public static void CheckReturnCode(int value)
		{
			if (value != 0) throw new SmartCardException(value);
		}
	}
}