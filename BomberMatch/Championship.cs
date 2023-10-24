namespace BomberMatch
{
	public sealed class Championship
	{
		private const int WinnerScore = 2;
		private const int DrawScore = 1;

		private readonly Dictionary<string, int> table = new Dictionary<string, int>();

		public void AddMatchResult(MatchResult matchResult)
		{
			if (matchResult.HasWinner)
			{
				AddBomberScore(matchResult.Winner, WinnerScore);
			}
			else if (matchResult.NoOneSurvivedDraw)
			{
				foreach (var bomber in matchResult.Bombers)
				{
					AddBomberScore(bomber.BomberName, DrawScore);
				}
			}
			else if (matchResult.TimeoutDraw)
			{
				foreach (var bomber in matchResult.AliveBombers)
				{
					AddBomberScore(bomber.BomberName, DrawScore);
				}
			}
		}

		public override string ToString()
		{
			var elements = table.Select(pair => $"{pair.Key}: {pair.Value}");
			return string.Join(Environment.NewLine, elements);
		}

		private void AddBomberScore(string bomberName, int score)
		{
			if (table.ContainsKey(bomberName))
			{
				table[bomberName] += score;
			}
			else
			{
				table[bomberName] = score;
			}
		}
	}
}
