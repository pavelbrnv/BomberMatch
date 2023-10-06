namespace BomberMatch.Bombers
{
	public class Killer : IBomber
	{
		private int detonationRadius;
		private int timeToDetonate;
		private List<Bomb> bombs = new List<Bomb>();
		private List<Bomber> bombers = new List<Bomber>();
		private List<Bomb> potentialBombKillers = new List<Bomb>();
		private List<Bomber> potentialBombers = new List<Bomber>();
		private Bomb nearestBomb;
		private Bomber nearestBomber;
		private Bomber IAm;
		public string Name => "Killer";

		public int Go(int[,] arena, int[,] bombers, int[] availableActions)
		{
			Clear();

			if (availableActions.Length == 0)
			{
				return (int)AcceptDeath();
			}

			ScanArena(arena, bombers);
			if (IsDangerous())
			{
				var action = FindNearestBombKiller().Escape();
				var intAction = (int)action;
				if (availableActions.Contains(intAction))
				{
					return intAction;
				}

				return availableActions[0]; // Weak place
			}

			if (FoundBomber())
			{
				var action1 = FindNearestBomber().PlaceBombAndEscape();
				var intAction = (int)action1;
				if (availableActions.Contains(intAction - 10))
				{
					return intAction;
				}

				return 10; // Weak place
			}

			var action2 = GoToAnyBomber();
			var intAction1 = (int)action2;

			if (availableActions.Contains(intAction1))
			{
				return intAction1;
			}

			return availableActions[0]; // Weak place
		}

		private void Clear()
		{
			bombs = new List<Bomb>();
			bombers = new List<Bomber>();
			potentialBombKillers = new List<Bomb>();
			potentialBombers = new List<Bomber>();
			nearestBomb = new Bomb();
			nearestBomber = new Bomber();
			IAm = new Bomber();
		}


		public void SetRules(int maxActionsNumber, int detonationRadius, int timeToDetonate)
		{
			this.detonationRadius = detonationRadius;
			this.timeToDetonate = timeToDetonate;
		}

		private Action AcceptDeath()
		{
			return Action.NoAndPlaceBomb;
		}

		private Action GoToAnyBomber()
		{
			var bomber = bombers[1];

			var xDist = Math.Abs(bomber.X - IAm.X);
			var yDist = Math.Abs(bomber.Y - IAm.Y);

			if (xDist > yDist)
			{
				if (IAm.Y > bomber.Y)
				{
					return Action.MoveUp;
				}

				return Action.MoveDown;
			}
			else
			{
				if (IAm.X > bomber.X)
				{
					return Action.MoveLeft;
				}

				return Action.MoveRight;
			}
		}

		private bool FoundBomber()
		{
			var skip = true;

			foreach (var bomber in bombers)
			{
				if (skip)
				{
					skip = false;
					continue;
				}

				if (IAm.Y == bomber.Y && Math.Abs(IAm.X - bomber.X) <= detonationRadius ||
				    IAm.X == bomber.X && Math.Abs(IAm.Y - bomber.Y) <= detonationRadius)
				{
					potentialBombers.Add(bomber);
				}
			}

			return potentialBombers.Count > 0;
		}

		private void ScanArena(int[,] arena, int[,] bombersCoords)
		{
			bombs = new List<Bomb>();

			for (var i = 0; i < arena.GetLength(0); i++)
			{
				for (var j = 0; j < arena.GetLength(1); j++)
				{
					if (arena[i, j] == -1 || arena[i, j] == 0)
					{
						continue;
					}

					var bomb = new Bomb(i, j, arena[i, j]);
					bombs.Add(bomb);
				}
			}

			bombers = new List<Bomber>();

			for (var i = 0; i < bombersCoords.GetLength(0); i++)
			{
				if (i == 0)
				{
					IAm = new Bomber(bombersCoords[i, 0], bombersCoords[i, 1]);
					bombers.Add(IAm);
				}
				else
				{
					var bomber = new Bomber(bombersCoords[i, 0], bombersCoords[i, 1]);
					bombers.Add(bomber);
				}
			}
		}

		private bool IsDangerous()
		{
			foreach (var bomb in bombs)
			{
				if (IAm.Y == bomb.Y && Math.Abs(IAm.X - bomb.X) <= detonationRadius ||
				    IAm.X == bomb.X && Math.Abs(IAm.Y - bomb.Y) <= detonationRadius)
				{
					potentialBombKillers.Add(bomb);
				}
			}

			return potentialBombKillers.Count > 0;
		}

		private Killer FindNearestBomber()
		{
			var min = int.MaxValue;
			var found = false;
			Bomber res = new Bomber();
			foreach (var bomber in potentialBombers)
			{
				if (Math.Abs(IAm.X - bomber.X) < min && IAm.Y == bomber.Y)
				{
					min = Math.Abs(IAm.X - bomber.X);
					res = bomber;
					found = true;
				}

				if (Math.Abs(IAm.Y - bomber.Y) < min && IAm.X == bomber.X)
				{
					min = Math.Abs(IAm.Y - bomber.Y);
					res = bomber;
					found = true;
				}
			}

			if (found)
			{
				nearestBomber = res;
				return this;
			}

			throw new Exception("Can't find nearest enemy bomber, but I know that he is somewhere here. Goodbye!");
		}

		private Killer FindNearestBombKiller()
		{
			var min = int.MaxValue;
			var found = false;
			Bomb res = new Bomb();
			foreach (var bomb in potentialBombKillers)
			{
				if (Math.Abs(IAm.X - bomb.X) < min && IAm.Y == bomb.Y)
				{
					min = Math.Abs(IAm.X - bomb.X);
					res = bomb;
					found = true;
				}

				if (Math.Abs(IAm.Y - bomb.Y) < min && IAm.X == bomb.X)
				{
					min = Math.Abs(IAm.Y - bomb.Y);
					res = bomb;
					found = true;
				}
			}

			if (found)
			{
				nearestBomb = res;
				return this;
			}

			throw new Exception("Can't find nearest bomb, but I know that it is somewhere here. Goodbye!");
		}

		private Action PlaceBombAndEscape()
		{
			if (IAm.Y == nearestBomber.Y)
			{
				if (IAm.X <= nearestBomber.X)
				{
					return Action.MoveLeftAndPlaceBomb;
				}

				return Action.MoveRightAndPlaceBomb;
			}

			if (IAm.X == nearestBomber.X)
			{
				if (IAm.Y <= nearestBomber.Y)
				{
					return Action.MoveDownAndPlaceBomb;
				}

				return Action.MoveUpAndPlaceBomb;
			}

			throw new Exception("Bugged bomb is about to blow in my hands, goodbye");
		}

		private Action Escape()
		{
			if (IAm.Y == nearestBomb.Y)
			{
				if (IAm.X <= nearestBomb.X)
				{
					return Action.MoveLeft;
				}

				return Action.MoveRight;
			}

			if (IAm.X == nearestBomb.X)
			{
				if (IAm.Y <= nearestBomb.Y)
				{
					return Action.MoveDown;
				}

				return Action.MoveUp;
			}

			throw new Exception("Can't escape the bomb, goodbye");
		}

		private struct Bomber
		{
			public int X { get; }
			public int Y { get; }

			public Bomber(int y, int x)
			{
				X = x;
				Y = y;
			}
		}

		private struct Bomb
		{
			public int X { get; }
			public int Y { get; }
			public int TimeToExplode { get; } // WeakPlace

			public Bomb(int y, int x, int timeToExplode)
			{
				X = x;
				Y = y;
				TimeToExplode = timeToExplode;
			}
		}

		private enum Action
		{
			No = 0,
			MoveUp = 1,
			MoveDown = 2,
			MoveLeft = 3,
			MoveRight = 4,
			NoAndPlaceBomb = 10,
			MoveUpAndPlaceBomb = 11,
			MoveDownAndPlaceBomb = 12,
			MoveLeftAndPlaceBomb = 13,
			MoveRightAndPlaceBomb = 14,
		}
	}
}