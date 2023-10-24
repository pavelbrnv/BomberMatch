namespace BomberMatch
{
	public sealed class MatchStateDumper : IMatchObserver, IDisposable
	{
		private readonly string fileName;

		private StreamWriter sw;
		private Dictionary<string, int> bomberNameToIndex;

		public MatchStateDumper(string fileName = "match-log.txt")
		{
			this.fileName = fileName;
		}

		public void StartMatch(uint bombDetonationRadius, uint bombTimeToDetonate, Arena arena)
		{
			sw = File.CreateText(fileName);

			sw.WriteLine($"R{bombDetonationRadius}");
			sw.WriteLine($"T{bombTimeToDetonate}");
			sw.WriteLine($"B{arena.AliveBombers.Count}");

			int i = 0;
			bomberNameToIndex = new Dictionary<string, int>();
			foreach (var name in arena.AliveBombers)
			{
				sw.WriteLine(name);
				bomberNameToIndex.Add(name, ++i);
			}

			var fields = arena.Fields;

			sw.WriteLine($"W{fields.GetLength(1)}");
			sw.WriteLine($"H{fields.GetLength(0)}");
			for (int y = 0; y < fields.GetLength(0); y++)
			{
				for (int x = 0; x < fields.GetLength(1); x++)
				{
					if (fields[y, x] == null)
						sw.Write("#");
					else if (fields[y, x].Bombers.Count == 0)
						sw.Write(".");
					else
						sw.Write(bomberNameToIndex[fields[y, x].Bombers.First().Name]);
				}
				sw.WriteLine();
			}
		}

		public void AddRound(MatchRound round)
		{
			var actionsCodes = new List<string>();

			foreach (var action in round.ValidatedActions)
			{
				var code = bomberNameToIndex[action.BomberName].ToString();

				if (action.RealAction.PlanBomb)
				{
					code += "B";
				}

				if (action.RealAction.Movement.HasValue)
				{
					switch (action.RealAction.Movement.Value)
					{
						case Direction.Up:
							code += "U";
							break;

						case Direction.Down:
							code += "D";
							break;

						case Direction.Left:
							code += "L";
							break;

						case Direction.Right:
							code += "R";
							break;
					}
				}

				if (code.Length > 1)
				{
					actionsCodes.Add(code);
				}
			}

			if (actionsCodes.Count == 0)
			{
				sw.WriteLine(".");
			}				
			else
			{
				sw.WriteLine(string.Join(",", actionsCodes.ToArray()));
			}
			
			sw.Flush();

			foreach (var mistake in round.Mistakes)
			{
				LogMistake(mistake);
			}
		}

		private void LogMistake(BomberMistake mistake)
		{
			Console.WriteLine($"{mistake.BomberName}: {mistake.Mistake.Message}");
		}

		public void EndMatch()
		{
			if (sw != null)
			{
				sw.Close();
				sw = null;
			}
		}

		public void Dispose()
		{
			EndMatch();
		}
	}
}
