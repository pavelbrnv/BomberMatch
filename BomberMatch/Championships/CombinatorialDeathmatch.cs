using BomberMatch.Observers;

namespace BomberMatch.Championships
{
	public sealed class CombinatorialDeathmatch
	{
		private readonly uint matchActionsNumber;
		private readonly uint bombDetonationRadius;
		private readonly uint bombTimeToDetonate;
		private readonly TimeSpan bomberActionTimeout;

		public CombinatorialDeathmatch(
			uint matchActionsNumber,
			uint bombDetonationRadius,
			uint bombTimeToDetonate,
			TimeSpan bomberActionTimeout)
		{
			this.matchActionsNumber = matchActionsNumber;
			this.bombDetonationRadius = bombDetonationRadius;
			this.bombTimeToDetonate = bombTimeToDetonate;
			this.bomberActionTimeout = bomberActionTimeout;
		}

		public void Bomb(string title, IReadOnlyList<string> mapsNames, IReadOnlyList<BomberDescriptor> bombersDescriptors, int oneMatchBombersNumber)
		{
			Directory.CreateDirectory(title);

			using (var sw = File.CreateText(Path.Combine(title, "championat.txt")))
			{
				var bombersNames = bombersDescriptors.Select(descriptor => BombersFactory.CreateBomber(descriptor).Name);
				sw.WriteLine(string.Join(";", bombersNames));

				var gameIndex = 1;

				var bombers = new IBomber[oneMatchBombersNumber];

				foreach (var mapName in mapsNames)
				{
					sw.WriteLine(mapName);

					var combs = MultiplyCombinations(GetCombinations(bombersDescriptors.Count, oneMatchBombersNumber));

					foreach (var comb in combs)
					{
						for (int i = 0; i < oneMatchBombersNumber; i++)
						{
							bombers[i] = BombersFactory.CreateBomber(bombersDescriptors[comb[i]]);
						}

						var arena = ArenasFactory.CreateArena(mapName);

						Console.WriteLine($"#{gameIndex} {mapName}: {string.Join(", ", bombers.Select(b => b.Name))}...");

						var matchLogName = $"{gameIndex:00000}.txt";
						using (var observer = new MatchStateDumper(Path.Combine(title, matchLogName)))
						{
							var match = new Match(
								arena: arena,
								observer: observer,
								bombers: bombers,
								matchActionsNumber: matchActionsNumber,
								bombDetonationRadius: bombDetonationRadius,
								bombTimeToDetonate: bombTimeToDetonate,
								bomberActionTimeout: bomberActionTimeout);

							try
							{
								var result = match.BombIt();

								sw.Write($"{matchLogName}");
								foreach (var bomber in result.Bombers)
								{
									sw.Write($";{bomber.BomberName};{(bomber.IsAlive ? 'A' : 'D')}");
								}

								sw.WriteLine();
							}
							catch (Exception ex)
							{
								Console.WriteLine($"ERROR: {ex.Message}");
							}
						}

						gameIndex++;
					}
				}
			}
		}

		private static int[][] GetCombinations(int n, int k)
		{
			if (n < k)
			{
				throw new ArgumentException("k cannot be grater that n");
			}

			var result = new List<int[]>();

			var value = new int[k];
			for (int i = 0; i < k; i++)
			{
				value[i] = i;
			}
			result.Add((int[])value.Clone());

			var enough = false;
			while (!enough)
			{
				enough = true;
				for (int i = k - 1; i >= 0; i--)
				{
					if (value[i] < n - k + i)
					{
						value[i]++;
						for (int j = i + 1; j < k; j++)
						{
							value[j] = value[j - 1] + 1;
						}
						result.Add((int[])value.Clone());
						enough = false;
						break;
					}
				}
			}
			return result.ToArray();
		}

		private static int[][] MultiplyCombinations(int[][] combs)
		{
			if (combs.Length == 0)
			{
				return combs;
			}

			var N = combs[0].Length;
			if (N != 4)
			{
				throw new NotImplementedException("MultiplyCombinations implemented only for 4");
			}

			var result = new int[combs.Length * 3][];
			for (int i = 0; i < combs.Length; i++)
			{
				result[i * 3] = new int[] { combs[i][0], combs[i][1], combs[i][2], combs[i][3] };
				result[i * 3 + 1] = new int[] { combs[i][0], combs[i][2], combs[i][3], combs[i][1] };
				result[i * 3 + 2] = new int[] { combs[i][0], combs[i][3], combs[i][1], combs[i][2] };
			}
			return result;
		}
	}
}
