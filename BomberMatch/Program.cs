using BomberMatch;

var championship = new Championship();

var maps = new[]
{
	Maps.Classic7x7B2,
	Maps.Andromeda17x17B4
};

var bombersNames = BombersFactory.GetBombersNames().ToArray();

var gameIndex = 1;
for (var i = 0; i < bombersNames.Length - 1; i++)
{
	for (var j = i + 1; j < bombersNames.Length; j++)
	{
		foreach (var map in maps)
		{
			for (var n = 0; n < 2; n++)
			{
				var bomber1 = BombersFactory.CreateBomber(bombersNames[i], string.Empty);
				var bomber2 = BombersFactory.CreateBomber(bombersNames[j], string.Empty);

				var arena = Arena.Build(map);

				Console.Write($"#{gameIndex}: {bomber1.Name} vs {bomber2.Name}...  ");

				using (var observer = new MatchStateDumper($"game_{gameIndex}_{bomber1.Name}_{bomber2.Name}"))
				{
					var match = new Match(
							arena: arena,
							observer: observer,
							bombers: new IBomber[] { bomber1, bomber2 },
							matchActionsNumber: 1000,
							bombDetonationRadius: 2,
							bombTimeToDetonate: 4,
							bomberActionTimeout: TimeSpan.FromSeconds(2));

					try
					{
						var result = match.BombIt();
						championship.AddMatchResult(result);

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
Console.WriteLine(championship);