namespace BomberMatch.Bombers
{
	public class TimBomber : IBomber
	{
		private int actionsRemain;
		private int detonationRadius;
		private int timeToDetonate;
		private Random r = new Random(1669);
		private static int MAX_ACTIONS_COUNT = 5;

		#region IBomber

		public string Name => "Tim";

		public void SetRules(int maxActionsNumber, int detonationRadius, int timeToDetonate)
		{
			this.actionsRemain = maxActionsNumber;
			this.detonationRadius = detonationRadius;
			this.timeToDetonate = timeToDetonate;
		}

		public int Go(int[,] arena, int[,] bombers, int[] availableActions)
		{
			try
			{
				actionsRemain--;

				var myPosX = bombers[0, 1];
				var myPosY = bombers[0, 0];
				var p = new CurrentStateProcessor(detonationRadius, timeToDetonate, arena, bombers);

				HashSet<int> actionsWithStay = availableActions.ToHashSet();
				actionsWithStay.Add(0);//вроде, в новой версии "оставаться на месте" будет входить в список вариантов
				//добавляем небольшую случайность
				availableActions = actionsWithStay
					.OrderBy(_ => r.Next())
					.OrderByDescending(act =>
					{
						var nextPos = p.MakeStep(myPosX, myPosY, act);
						return p.GetActionsCountFromCell(nextPos.Item1, nextPos.Item2);
					})
					.ToArray();
				var availableMoves = availableActions.Where(action => action != 0);// именно когда двигаемся - не стоим на месте

				var safeActions = availableActions.Where(act => 
				{
					var nextPos = p.MakeStep(myPosX, myPosY, act);
					return !p.CanExplodeThisStep(nextPos.Item1, nextPos.Item2);
				}).ToList();
				var halfSafeActions = availableActions.Where(act =>
				{
					var nextPos = p.MakeStep(myPosX, myPosY, act);
					return !p.WillExplodeThisStep(nextPos.Item1, nextPos.Item2);
				}).ToList();

				// Если нет полностью безопасных ходов, рассмотрим хотя бы те, где полубезопасно
				if (safeActions.Count == 0)
				{
					safeActions = halfSafeActions;
				}
				// Если нет ходов, при которых не подорвёшься
				if (safeActions.Count == 0)
				{
					// точно ставим бомбу

					// пытаемся подойти вплотную к сопернику.
					foreach (var move in availableMoves)
					{
						var newPos = p.MakeStep(myPosX, myPosY, move);
						if (p.GetNearestOpponentDistance(newPos.Item1, newPos.Item2) == 0)
						{
							return move + 10;
						}
					}
					// !!! попытаться подойти ближе к сопернику.
					return availableActions[0] + 10;
				}
				if (safeActions.Count == 1)
				{
					return safeActions[0];
				}

				//рядом бомба - просто убегаем
				if (p.CanExplodeThisStep(myPosX, myPosY))
				{
					var tmp = p.MakeStepIfCanExplodeThisStep(safeActions);
					if (tmp != null) { return tmp.Value; }

					tmp = p.MakeStepIfCanExplodeThisStep(halfSafeActions);
					if (tmp != null) { return tmp.Value; }
				}

				if (p.IsUnderBlast(myPosX, myPosY))
				{
					// пробуем найти более безопасное место

					var tmp = p.MakeStepIfBombIsNear(safeActions);
					if (tmp != null) { return tmp.Value; }

					tmp = p.MakeStepIfBombIsNear(halfSafeActions);
					if (tmp != null) { return tmp.Value; }

					// а если не нашли укромное место, то забиваем и стоим.
					return 0;
				}

				var currentOpponentDistance = p.GetNearestOpponentDistance();
				if (currentOpponentDistance <= detonationRadius)
				{
					//если соперник рядом
					var safeActionsN = availableActions.Where(act =>
					{
						var nextPos = p.MakeStep(myPosX, myPosY, act);
						return !p.CanExplodeNSteps(nextPos.Item1, nextPos.Item2, 2);
					}).ToList();

					var tmp = p.MakeStepIfOpponentIsNear(safeActionsN, true);
					if (tmp != null) { return tmp.Value; }

					tmp = p.MakeStepIfOpponentIsNear(halfSafeActions, false);
					if (tmp != null) { return tmp.Value; }
				}
				else
				{
					// если пока что много соперников
					if (p.OpponentsCount > 1 && actionsRemain > 50)
					{
						var currentActionsCount = availableActions.Length;
						// убегаем и не ставим
						foreach (var action in safeActions)
						{
							var nextPos = p.MakeStep(myPosX, myPosY, action);
							if (p.GetNearestOpponentDistance(nextPos.Item1, nextPos.Item2) > currentOpponentDistance &&
								p.GetActionsCountFromCell(nextPos.Item1, nextPos.Item2) >= MAX_ACTIONS_COUNT - 1) // это чтобы не забиваться в углы
							{
								return action;
							}
						}
						foreach (var action in safeActions)
						{
							var nextPos = p.MakeStep(myPosX, myPosY, action);
							if (p.GetNearestOpponentDistance(nextPos.Item1, nextPos.Item2) >= currentOpponentDistance &&
								p.GetActionsCountFromCell(nextPos.Item1, nextPos.Item2) >= MAX_ACTIONS_COUNT - 1) // это чтобы не забиваться в углы
							{
								return action;
							}
						}
						foreach (var action in safeActions)
						{
							var nextPos = p.MakeStep(myPosX, myPosY, action);
							if (p.GetNearestOpponentDistance(nextPos.Item1, nextPos.Item2) >= currentOpponentDistance)
							{
								return action;
							}
						}
					}

					foreach (var action in availableActions)
					{
						var newPos = MakeStep(myPosX, myPosY, action);

						if (p.GetNearestOpponentDistance(newPos.Item1, newPos.Item2) < currentOpponentDistance &&
							p.GetNearestBombDistance(newPos.Item1, newPos.Item2) > detonationRadius)
						{
							bool makeBomb = currentOpponentDistance <= detonationRadius;
							return action + (makeBomb ? 10 : 0);
						}
					}
				}

				var safeAction = GenerateSafeStep(p, myPosX, myPosY, availableActions);
				//!!! добавить логику, что если уже нет выхода, то ставить бомбу
				// добавить счётчик, через сколько взорвётся текущая бомба
				// добавить проверку, что поле, куда идём, будет взорвано позже, чем то, где мы стоим
				// ставить бомбу когда соперник находится ближе
				// обработать ситуацию, когда можно успеть убежать из-под волны:
				// Я 0 0 0
				// 2 2 0 0
				// если соперников много, то не спешим сближаться
				// ходить лучше в те места, откуда будет больше доступных ходов
				return availableActions.Contains(safeAction % 10) ? safeAction : 0;//проверка на всякий случай
			}
			catch
			{
				return availableActions[r.Next(availableActions.Length)];
			}
		}

		#endregion

		private int GenerateSafeStep(CurrentStateProcessor p, int myPosX, int myPosY, int[] availableActions)
		{
			//можно ещё добавить логику, чтобы среди всех вариантов выбирался самый адекватный, например, тот, из которого будет больше путей для отступления (не у стены)
			var nearestBombDist = p.GetNearestBombDistance();
			foreach (var action in availableActions)
			{
				var newPos = MakeStep(myPosX, myPosY, action);

				if (p.GetNearestBombDistance(newPos.Item1, newPos.Item2) > detonationRadius)
					return action;
			}
			foreach (var action in availableActions)
			{
				var newPos = MakeStep(myPosX, myPosY, action);

				if (p.GetNearestBombDistance(newPos.Item1, newPos.Item2) > nearestBombDist)
					return action;
			}

			// а если не нашли укромное место, то забиваем и стоим.
			return p.IsUnderBlast(myPosX, myPosY) ? 10 : 0;
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

		public class CurrentStateProcessor
		{
			#region Private properties

			private int detonationRadius;
			private int timeToDetonate;

			private int[,] arena;
			private List<Point> opponents;
			public int OpponentsCount => opponents.Count;
			private int xLen => arena.GetLength(0);
			private int yLen => arena.GetLength(1);
			private readonly int myPosX;
			private readonly int myPosY;

			#endregion

			public CurrentStateProcessor(int detonationRadius, int timeToDetonate, int[,] arena, int[,] bombers)
			{
				this.detonationRadius = detonationRadius;
				this.timeToDetonate = timeToDetonate;
				this.arena = arena;
				this.opponents = new List<Point>();
				for (int x = 1; x < bombers.GetLength(0); x++)
				{
					opponents.Add(new Point { X = bombers[x, 1], Y = bombers[x, 0] });
				}

				myPosX = bombers[0, 1];
				myPosY = bombers[0, 0];
				ArenaDistances = GetArenaDistances(new List<Point> { new Point() { X = bombers[0, 1], Y = bombers[0, 0] } });
				ExplosionZone = GetExplosionZone(false);
				ExplosionKamikadzeZone = GetExplosionZone(true);
			}

			public readonly int[,] ArenaDistances;
			public readonly bool[,] ExplosionZone;
			public readonly bool[,] ExplosionKamikadzeZone;

			#region Public Methods

			#region Check walls, bombs, opponents

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

				return opponents.Any(opponent => opponent.X == x && opponent.Y == y);
			}

			public bool IsAvailable(int x, int y)
			{
				if (x < 0 || y < 0 || x >= arena.GetLength(0) || y >= arena.GetLength(1))
					return false;

				return arena[y, x] == 0;
			}

			public bool IsAvailable(Point p)
			{
				return IsAvailable(p.X, p.Y);
			}

			#endregion

			#region Making steps for different actions

			public Tuple<int, int> MakeStep(int x, int y, int action)
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

			public int? MakeStepIfCanExplodeThisStep(List<int> actions)
			{
				foreach (var action in actions)
				{
					var newPos = MakeStep(myPosX, myPosY, action);
					if (!CanExplodeThisStep(newPos.Item1, newPos.Item2))
					{
						return action;
					}
				}
				return null;
			}

			public int? MakeStepIfBombIsNear(List<int> actions)
			{
				var currentBombDistance = GetNearestBombDistance();
				foreach (var move in actions)
				{
					var newPos = MakeStep(myPosX, myPosY, move);
					if (GetNearestBombDistance(newPos.Item1, newPos.Item2) > currentBombDistance)
					{
						return move;
					}
				}
				// затем ищем место, идентичное по безопасности
				foreach (var move in actions)
				{
					var newPos = MakeStep(myPosX, myPosY, move);
					if (GetNearestBombDistance(newPos.Item1, newPos.Item2) == currentBombDistance)
					{
						return move;
					}
				}

				return null;
			}

			public int? MakeStepIfOpponentIsNear(List<int> actions, bool makeBomb)
			{
				var currentOpponentDistance = GetNearestOpponentDistance();
				foreach (var action in actions)
				{
					var newPos = MakeStep(myPosX, myPosY, action);
					if (GetNearestOpponentDistance(newPos.Item1, newPos.Item2) > currentOpponentDistance &&
						GetNearestBombDistance(newPos.Item1, newPos.Item2) > detonationRadius)
					{
						return makeBomb ? action + 10 : action;
					}
				}
				return null;
			}

			#endregion

			/// <summary>
			/// Взорвётся ли клетка, если никто не поставит бомбу
			/// </summary>
			public bool WillExplodeThisStep(int x, int y)
			{
				return ExplosionZone[x, y];
			}

			/// <summary>
			/// Взорвётся ли клетка, если все игроки неожиданно станут камикадзе
			/// </summary>
			public bool CanExplodeThisStep(int x, int y)
			{
				return ExplosionKamikadzeZone[x, y];
			}

			/// <summary>
			/// Взорвётся ли клетка через N шагов, если все игроки неожиданно станут камикадзе
			/// </summary>
			public bool CanExplodeNSteps(int x, int y, int stepsCount)
			{
				var zone = GetExplosionZoneNSteps(stepsCount, true);
				return zone[x, y];
			}

			/// <summary>
			/// Получаем зону, которая точно будет покрыта взрывом в конце шага.
			/// </summary>
			/// <param name="opponentsAreKamikadze">Стоит ли предполагать, что все игроки на этом ходе поставят бомбу</param>
			private bool[,] GetExplosionZone(bool opponentsAreKamikadze = false)
			{
				bool[,] explodedCells = new bool[xLen, yLen];

				var unexplodedBombs = GetBombsList().ToHashSet();

				//1) найти бомбы, которые сейчас взорвутся
				var bombsToExplodeNow = unexplodedBombs.Where(bomb => arena[bomb.Y, bomb.X] == 1).ToHashSet();

				//!!! проверить, что не происходит зацикливания
				int i = 0;// это временно и на всякий случай
				while (bombsToExplodeNow.Count > 0 && i++ < xLen*yLen)
				{
					foreach (var bomb in bombsToExplodeNow.ToList())
					{
						//2) пометить точки, на которых они стоят, как взрывные. Удалить бомбу
						explodedCells[bomb.X, bomb.Y] = true;
						bombsToExplodeNow.Remove(bomb);

						//3) пройтись по всем соседним, пометить как взрывные
						//если есть бомба в радиусе, с каждой такой перейти к п.2
						foreach (var point in GetPointsByRadius(bomb.X, bomb.Y, detonationRadius))
						{
							explodedCells[point.X, point.Y] = true;
							if (IsBomb(point.X, point.Y))//!!! вот тут можно добавить логику, что если на точке стоит соперник, то он может поставить бомбу, и тогда зона будет под атакой
							{
								bombsToExplodeNow.Add(point);
							}
							if (opponentsAreKamikadze && IsOpponent(point.X, point.Y))
							{
								bombsToExplodeNow.Add(point);
							}
						}
					}
				}

				return explodedCells;
			}

			/// <summary>
			/// Получаем зону, которая точно будет покрыта взрывом в конце шага.
			/// </summary>
			/// <param name="opponentsAreKamikadze">Стоит ли предполагать, что все игроки на этом ходе поставят бомбу</param>
			private bool[,] GetExplosionZoneNSteps(int stepsCount, bool opponentsAreKamikadze = false)
			{
				bool[,] explodedCells = new bool[xLen, yLen];

				var unexplodedBombs = GetBombsList().ToHashSet();

				//1) найти бомбы, которые сейчас взорвутся
				var bombsToExplodeInFewSteps = unexplodedBombs.Where(bomb => arena[bomb.Y, bomb.X] <= stepsCount).ToHashSet();

				//!!! проверить, что не происходит зацикливания
				int i = 0;// это временно и на всякий случай
				while (bombsToExplodeInFewSteps.Count > 0 && i++ < xLen * yLen)
				{
					foreach (var bomb in bombsToExplodeInFewSteps.ToList())
					{
						//2) пометить точки, на которых они стоят, как взрывные. Удалить бомбу
						explodedCells[bomb.X, bomb.Y] = true;
						bombsToExplodeInFewSteps.Remove(bomb);

						//3) пройтись по всем соседним, пометить как взрывные
						//если есть бомба в радиусе, с каждой такой перейти к п.2
						foreach (var point in GetPointsByRadius(bomb.X, bomb.Y, detonationRadius))
						{
							explodedCells[point.X, point.Y] = true;
							if (IsBomb(point.X, point.Y))//!!! вот тут можно добавить логику, что если на точке стоит соперник, то он может поставить бомбу, и тогда зона будет под атакой
							{
								bombsToExplodeInFewSteps.Add(point);
							}
							//!!! вот тут надо бы ещё учесть, что соперники могут двигаться. Но это долго
							if (opponentsAreKamikadze && IsOpponent(point.X, point.Y))
							{
								bombsToExplodeInFewSteps.Add(point);
							}
						}
					}
				}

				return explodedCells;
			}

			/// <summary>
			/// Если под взрывом, но не обязательно в этот ход
			/// </summary>
			public bool IsUnderBlast(int x, int y)
			{
				//!!! вот тут добавить логику, что если ещё успеваем убежать, то ок
				foreach (var nearPoint in GetPointsByRadius(x, y, detonationRadius))
				{
					if (IsBomb(nearPoint.X, nearPoint.Y))
					{
						return true;
					}
				}

				return false;
			}

			public int GetActionsCountFromCell(int x, int y)
			{
				int count = 1;//всегда можем остаться на месте
				if (IsAvailable(x - 1, y)) count++;
				if (IsAvailable(x + 1, y)) count++;
				if (IsAvailable(x, y - 1)) count++;
				if (IsAvailable(x, y + 1)) count++;
				return count;
			}

			private IEnumerable<Point> GetPointsByRadius(int x, int y, int radius)
			{
				for (int i = x - 1; i >= x - radius; i--)
				{
					if (IsWall(i, y))
						break;
					yield return new Point { X = i, Y = y };
				}
				for (int i = x + 1; i <= x + radius; i++)
				{
					if (IsWall(i, y))
						break;
					yield return new Point { X = i, Y = y };
				}
				for (int j = y - 1; j >= y - radius; j--)
				{
					if (IsWall(x, j))
						break;
					yield return new Point { X = x, Y = j };
				}
				for (int j = y + 1; j <= y + radius; j++)
				{
					if (IsWall(x, j))
						break;
					yield return new Point { X = x, Y = j };
				}
			}

			#endregion

			private List<Point> GetBombsList()
			{
				List<Point> bombs = new List<Point>();
				for (int i = 0; i < arena.GetLength(0); i++)
				{
					for (int j = 0; j < arena.GetLength(1); j++)
					{
						if (IsBomb(i, j))
							bombs.Add(new Point { X = i, Y = j });
					}
				}
				return bombs;
			}

			/// <summary>
			/// Считает расстояние от текущего положения игрока до ближайшей бомбы
			/// </summary>
			public int GetNearestBombDistance()
			{
				var bombs = GetBombsList();
				return bombs.Count > 0 ? bombs.Min(bomb => ArenaDistances[bomb.X, bomb.Y]) : int.MaxValue;
			}

			/// <summary>
			/// Считает расстояние от заданной точки до ближайшей бомбы
			/// </summary>
			public int GetNearestBombDistance(int x, int y)
			{
				var arenaFromPoint = GetArenaDistances(new List<Point> { new Point { X = x, Y = y } });
				var bombs = GetBombsList();
				return bombs.Count > 0 ? bombs.Min(bomb => arenaFromPoint[bomb.X, bomb.Y]) : int.MaxValue;
			}

			/// <summary>
			/// Считает расстояние от текущего положения игрока до ближайшего соперника
			/// </summary>
			/// <returns></returns>
			public int GetNearestOpponentDistance()
			{
				return opponents.Min(op => ArenaDistances[op.X, op.Y]);
			}

			/// <summary>
			/// Считает расстояние от заданной точки до ближайшего соперника
			/// </summary>
			public int GetNearestOpponentDistance(int x, int y)
			{
				var arenaFromPoint = GetArenaDistances(new List<Point> { new Point { X = x, Y = y } });
				return opponents.Min(bomb => arenaFromPoint[bomb.X, bomb.Y]);
			}

			private int[,] GetArenaDistances(List<Point> startPositions)
			{
				int[,] arenaDistances = new int[xLen, yLen];
				int currentDist = 0;
				List<Point> fieldsToProcess = startPositions;
				List<Point> fieldsToVisit;

				while (fieldsToProcess.Count != 0)
				{
					fieldsToVisit = new List<Point>();
					currentDist++;
					foreach (var point in fieldsToProcess)
					{
						//получаем все возможные пути
						var availableCells = GetAvailableNeighbourCells(point);

						//теперь надо оставить те пути, которые не посещали
						foreach (var cell in availableCells)
						{
							if (arenaDistances[cell.X, cell.Y] == 0 && !startPositions.Contains(cell))
							{
								arenaDistances[cell.X, cell.Y] = currentDist;
								if (IsAvailable(cell))
								{
									fieldsToVisit.Add(cell);
								}
							}
						}
					}
					fieldsToProcess = fieldsToVisit;
				}
				return arenaDistances;
			}
			private List<Point> GetAvailableNeighbourCells(Point point)
			{
				List<Point> neighbours = new List<Point>();
				CheckAndAdd(point.X - 1, point.Y, neighbours);
				CheckAndAdd(point.X + 1, point.Y, neighbours);
				CheckAndAdd(point.X, point.Y - 1, neighbours);
				CheckAndAdd(point.X, point.Y + 1, neighbours);

				return neighbours;
			}
			/// <summary>
			/// Проверяем, нет ли стены на данном поле, если нет, добавляем точку в список points
			/// </summary>
			private void CheckAndAdd(int x, int y, List<Point> points)
			{
				if (IsAvailable(x, y) || IsBomb(x, y))
				{
					points.Add(new Point { X = x, Y = y });
				}
			}
		}

		public struct Point
		{
			public int X;
			public int Y;
		}
	}
}
