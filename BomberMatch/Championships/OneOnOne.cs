namespace BomberMatch.Championships
{
	public sealed class OneOnOne
	{
		private readonly uint matchActionsNumber;
		private readonly uint bombDetonationRadius;
		private readonly uint bombTimeToDetonate;
		private readonly TimeSpan bomberActionTimeout;
		private readonly MatchObserverCreator createMatchObserver;

		public OneOnOne(
			uint matchActionsNumber,
			uint bombDetonationRadius,
			uint bombTimeToDetonate,
			TimeSpan bomberActionTimeout,
			MatchObserverCreator createMatchObserver)
		{
			this.matchActionsNumber = matchActionsNumber;
			this.bombDetonationRadius = bombDetonationRadius;
			this.bombTimeToDetonate = bombTimeToDetonate;
			this.bomberActionTimeout = bomberActionTimeout;
			this.createMatchObserver = createMatchObserver;
		}

		public void Bomb(IReadOnlyList<string> mapsNames, IReadOnlyList<BomberDescriptor> bombers)
		{
			var series = new SeriesRating();

			var gameIndex = 1;
			for (var i = 0; i < bombers.Count - 1; i++)
			{
				for (var j = i + 1; j < bombers.Count; j++)
				{
					foreach (var mapName in mapsNames)
					{
						for (var n = 0; n < 2; n++)
						{
							var bomber1 = BombersFactory.CreateBomber(bombers[i]);
							var bomber2 = BombersFactory.CreateBomber(bombers[j]);

							if (n % 2 == 1)
							{
								(bomber1, bomber2) = (bomber2, bomber1);
							}

							var arena = ArenasFactory.CreateArena(mapName);

							Console.Write($"#{gameIndex} {mapName}: {bomber1.Name} vs {bomber2.Name}...  ");
							
							using (var observer = createMatchObserver(gameIndex, mapName, bomber1.Name, bomber2.Name))
							{
								var match = new Match(
										arena: arena,
										observer: observer,
										bombers: new IBomber[] { bomber1, bomber2 },
										matchActionsNumber: matchActionsNumber,
										bombDetonationRadius: bombDetonationRadius,
										bombTimeToDetonate: bombTimeToDetonate,
										bomberActionTimeout: bomberActionTimeout);

								try
								{
									var result = match.BombIt();
									series.AddMatchResult(result);

									Console.WriteLine(result);
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

			Console.WriteLine(string.Empty);
			Console.WriteLine("--- CHAMPIONSHIP RESULTS ---");
			Console.WriteLine(series);
		}
	}
}
