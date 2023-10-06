namespace BomberMatch
{
	public sealed class Bomb
	{
		public Bomb(uint detonationRadius, uint timeToDetonate)
		{
			DetonationRadius = detonationRadius;
			TimeToDetonate = timeToDetonate;
		}

		public uint DetonationRadius { get; }

		public uint TimeToDetonate { get; private set; }

		public bool Bang => TimeToDetonate == 0;

		public void Tick()
		{
			TimeToDetonate--;
		}
	}
}
