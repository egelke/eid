
namespace Egelke.Eid.SmartCard
{
	public partial class SmartCardReader
	{
		private SmartCardReader(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}