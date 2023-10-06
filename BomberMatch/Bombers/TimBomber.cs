namespace BomberMatch.Bombers
{
	public class TimBomber : IBomber
	{
		private int detonationRadius;
		private int timeToDetonate;
		private Random r = new Random(1669);

		#region IBomber

		public string Name => "Tim";

		public void SetRules(int maxActionsNumber, int detonationRadius, int timeToDetonate)
		{
			this.detonationRadius = detonationRadius;
			this.timeToDetonate = timeToDetonate;
		}

		public int Go(int[,] arena, int[,] bombers, int[] availableActions)
		{
			try
			{
				var myPosX = bombers[0, 1];
				var myPosY = bombers[0, 0];
				var p = new CurrentStepProcessor(detonationRadius, timeToDetonate, arena, bombers);

				//рядом бомба - просто убегаем
				var currentBombDistance = p.GetDistanceToBomb(myPosX, myPosY);
				if (p.GetDistanceToBomb(myPosX, myPosY) <= detonationRadius)
				{
					foreach (var action in availableActions)
					{
						var newPos = MakeStep(myPosX, myPosY, action);
						if (p.GetDistanceToBomb(newPos.Item1, newPos.Item2) > currentBombDistance)
							return action;
					}
					// а если не нашли укромное место, то забиваем и стоим.
					return 0;
				}

				//иначе - идём в сторону соперника

				bool makeBomb = false;

				var currentOpponentDistance = p.GetDistanceToOpponent(myPosX, myPosY);
				foreach (var action in availableActions)
				{
					var newPos = MakeStep(myPosX, myPosY, action);
					//если соперник рядом
					if (currentOpponentDistance <= detonationRadius)
					{
						makeBomb = true;
					}

					if (p.GetDistanceToOpponent(newPos.Item1, newPos.Item2) < currentOpponentDistance &&
						p.GetDistanceToBomb(newPos.Item1, newPos.Item2) > detonationRadius)
					{
						return action + (makeBomb ? 10 : 0);
					}
				}

				var safeAction = GenerateSafeStep(p, myPosX, myPosY, availableActions);
				return availableActions.Contains(safeAction) ? safeAction + (makeBomb ? 10 : 0) : 0;//проверка на всякий случай
			}
			catch
			{
				var myActions = new int[availableActions.Length + 1];
				myActions[0] = 0;
				availableActions.CopyTo(myActions, 1);

				return myActions[r.Next(myActions.Length)];
			}
		}

		private int GenerateSafeStep(CurrentStepProcessor p, int myPosX, int myPosY, int[] availableActions)
		{
			var nearestBombDist = p.GetDistanceToBomb(myPosX, myPosY);
			foreach (var action in availableActions)
			{
				var newPos = MakeStep(myPosX, myPosY, action);

				if (p.GetDistanceToBomb(newPos.Item1, newPos.Item2) > detonationRadius)
					return action;
			}
			foreach (var action in availableActions)
			{
				var newPos = MakeStep(myPosX, myPosY, action);

				if (p.GetDistanceToBomb(newPos.Item1, newPos.Item2) > nearestBombDist)
					return action;
			}

			// а если не нашли укромное место, то забиваем и стоим.
			return 0;
		}

		private Tuple<int, int> MakeStep(int x, int y, int action)
		{
			switch (action)
			{
				case 0:
					return new Tuple<int, int>(x, y);
				case 1://up
					return new Tuple<int, int>(x, y - 1);
				case 2://down
					return new Tuple<int, int>(x, y + 1);
				case 3://left
					return new Tuple<int, int>(x - 1, y);
				case 4://right
					return new Tuple<int, int>(x + 1, y);
				default:
					return new Tuple<int, int>(x, y);
			}
		}

		#endregion

		public class CurrentStepProcessor
		{
			private int detonationRadius;
			private int timeToDetonate;
			private int[,] arena;
			private Tuple<int, int> opponentPos;

			public CurrentStepProcessor(int detonationRadius, int timeToDetonate, int[,] arena, int[,] bombers)
			{
				this.detonationRadius = detonationRadius;
				this.timeToDetonate = timeToDetonate;
				this.arena = arena;
				this.opponentPos = new Tuple<int, int>(bombers[1, 1], bombers[1, 0]);
			}

			public bool IsWall(int x, int y)
			{
				if (x < 0 || y < 0 || x >= arena.GetLength(0) || y >= arena.GetLength(1))
					return true;

				return arena[y, x] == -1;
			}
			public bool IsBomb(int x, int y)
			{
				if (x < 0 || y < 0 || x >= arena.GetLength(0) || y >= arena.GetLength(1))
					return false;

				return arena[y, x] > 0;
			}
			public bool IsOpponent(int x, int y)
			{
				if (x < 0 || y < 0 || x >= arena.GetLength(0) || y >= arena.GetLength(1))
					return false;

				return opponentPos.Item1 == x && opponentPos.Item2 == y;
			}

			public int GetDistanceToBomb(int x, int y, int currentDist = 0)
			{
				if (IsBomb(x, y)) return currentDist;
				if (IsWall(x, y)) return int.MaxValue;

				var xMax = arena.GetLength(0);
				var yMax = arena.GetLength(1);
				if (currentDist > xMax + yMax) return currentDist;

				//это медленно работает, зато быстро кодится
				var minX = Math.Min(GetDistanceToBomb(x - 1, y, currentDist + 1),
					GetDistanceToBomb(x + 1, y, currentDist + 1));
				var minY = Math.Min(GetDistanceToBomb(x, y - 1, currentDist + 1),
					GetDistanceToBomb(x, y + 1, currentDist + 1));
				return Math.Min(minX, minY);
			}

			public int GetDistanceToOpponent(int x, int y, int currentDist = 0)
			{
				if (IsOpponent(x, y)) return currentDist;
				if (IsWall(x, y)) return int.MaxValue;

				var xMax = arena.GetLength(0);
				var yMax = arena.GetLength(1);
				if (currentDist > xMax + yMax) return currentDist;

				//это медленно работает, зато быстро кодится
				var minX = Math.Min(GetDistanceToOpponent(x - 1, y, currentDist + 1),
					GetDistanceToOpponent(x + 1, y, currentDist + 1));
				var minY = Math.Min(GetDistanceToOpponent(x, y - 1, currentDist + 1),
					GetDistanceToOpponent(x, y + 1, currentDist + 1));
				return Math.Min(minX, minY);
			}
		}
	}
}
