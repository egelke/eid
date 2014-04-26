namespace Egelke.Eid.SmartCard.WinScard
{
	public enum CardState
	{
		/// <summary>
		/// The driver is unaware of the current state of the reader.
		/// </summary> 
		Unknown = 0,

		/// <summary>
		/// There is no card in the reader.
		/// </summary>
		Absent = 1,

		/// <summary>
		/// There is a card is present in the reader, but that it has not been moved into position for use.
		/// </summary>
		Present = 2,

		/// <summary>
		/// There is a card in the reader in position for use. The card is not powered.
		/// </summary>
		Swallowed = 3,

		/// <summary>
		/// There is power is being provided to the card, but the Reader Driver is unaware of the mode of the card.
		/// </summary>
		Powered = 4,

		/// <summary>
		/// The card has been reset and is awaiting PTS negotiation.
		/// </summary>
		Negotiable = 5,

		/// <summary>
		/// The card has been reset and specific communication protocols have been established.
		/// </summary>
		Specific = 6
	}
}