namespace BomberMatch
{
	public sealed class MatchResult
	{
		#region Ctor

		public MatchResult(IReadOnlyList<BomberResult> bombers)
		{
			Bombers = bombers;
		}

		#endregion

		#region Properties

		public IReadOnlyList<BomberResult> Bombers { get; }

		public IEnumerable<BomberResult> AliveBombers => Bombers.Where(bomber => bomber.IsAlive);

		public IEnumerable<BomberResult> DeadBombers => Bombers.Where(bomber => !bomber.IsAlive);

		public bool NoOneSurvivedDraw => Bombers.All(bomber => !bomber.IsAlive);

		public bool TimeoutDraw => AliveBombers.Count() > 1;

		public bool HasWinner => AliveBombers.Count() == 1;

		public string Winner => AliveBombers.Select(bomber => bomber.BomberName).Single();

		#endregion

		#region Overrides

		public override string ToString()
		{
			if (NoOneSurvivedDraw)
			{
				return "Draw! No one survived";
			}
			if (TimeoutDraw)
			{
				return "Draw! Timeout";
			}

			return $"The winner is ... {Winner}";
		}

		#endregion

		#region Nested

		public sealed class BomberResult
		{
			public BomberResult(string bomberName, bool isAlive, int lastActionNumber)
			{
				BomberName = bomberName;
				IsAlive = isAlive;
				LastActionNumber = lastActionNumber;
			}

			public string BomberName { get; }

			public bool IsAlive { get; }

			public int LastActionNumber { get; }
		}

		public sealed class Builder
		{
			private readonly List<BomberResult> bombers = new();

			public void AddBomberResult(string bomberName, bool isAlive, int lastActionNumber)
			{
				bombers.Add(new BomberResult(bomberName, isAlive, lastActionNumber));
			}

			public MatchResult Build()
			{
				return new MatchResult(bombers);
			}
		}

		#endregion
	}
}
