namespace BomberMatch
{
	public class Field
	{
		#region Fields

		private HashSet<Man> bombers = new();

		#endregion

		#region Ctors

		public Field(bool allowRespawn)
		{
			AllowRespawn = allowRespawn;
		}

		public Field()
			: this(false)
		{
		}

		#endregion

		#region Properties

		public bool AllowRespawn { get; }

		public Bomb? Bomb { get; private set; }

		public bool HasBomb => Bomb != null;

		public IReadOnlyCollection<Man> Bombers => bombers;
		
		public Field? UpField { get; internal set; }
		
		public Field? DownField { get; internal set; }
		
		public Field? LeftField { get; internal set; }
		
		public Field? RightField { get; internal set; }
		
		#endregion

		#region Public methods

		public void PlantBomb(Bomb bomb)
		{
			if (Bomb == null || bomb.TimeToDetonate < Bomb.TimeToDetonate)
			{
				Bomb = bomb;
			}
		}
		
		public bool AddBomber(Man bomber)
		{
			return bombers.Add(bomber);
		}

		public bool RemoveBomber(Man bomber)
		{
			return bomber.IsAlive && bombers.Remove(bomber);
		}
		
		public void Tick()
		{
			if (Bomb == null)
			{
				return;
			}

			Bomb.Tick();

			if (Bomb.Bang)
			{
				ExplodeBomb();
			}
		}

		public void Bang()
		{
			KillAllBombers();

			ExplodeBomb();
		}

		#endregion

		#region Private methods

		private void KillAllBombers()
		{
			foreach (var bomber in Bombers)
			{
				bomber.Kill();
			}
		}

		private void ExplodeBomb()
		{
			if (Bomb == null)
			{
				return;
			}

			var radius = Bomb.DetonationRadius;

			Bomb = null;

			KillAllBombers();

			ExplodeByRadius(radius, field => field?.UpField);
			ExplodeByRadius(radius, field => field?.DownField);
			ExplodeByRadius(radius, field => field?.LeftField);
			ExplodeByRadius(radius, field => field?.RightField);
		}

		private void ExplodeByRadius(uint radius, Func<Field?, Field?> getNextField)
		{
			var field = this;
			for (var i = 0; i < radius; i++)
			{
				field = getNextField(field);
				if (field == null)
				{
					return;
				}
				field.Bang();
			}
		}

		#endregion
	}
}
