namespace BomberMatch
{
	public sealed class Man
	{
		#region Ctor

		public Man(string name, uint bombDetonationRadius, uint bombTimeToDetonate, Field initialField)
		{
			Name = name;
			BombDetonationRadius = bombDetonationRadius;
			BombTimeToDetonate = bombTimeToDetonate;

			CurrentField = initialField;	

			IsAlive = true;
		}

		#endregion

		#region Properties

		public string Name { get; }

		public uint BombDetonationRadius { get; }

		public uint BombTimeToDetonate { get; }

		public Field CurrentField { get; private set; }

		public bool IsAlive { get; private set; }

		#endregion

		#region Public methods

		public void MoveToField(Field field)
		{
			CurrentField = field;
		}

		public void Kill()
		{
			IsAlive = false;
		}

		public Bomb GetBomb()
		{
			return new Bomb(BombDetonationRadius, BombTimeToDetonate);
		}

		#endregion
	}
}
