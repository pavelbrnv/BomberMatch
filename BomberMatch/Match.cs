namespace BomberMatch
{
	public sealed class Match
	{
		#region Consts

		private const int BompPlantingSign = 10;

		private const int FieldWallCode = -1;
		private const int FieldEmptyCode = 0;

		private const int MoveNoneCode = 0;
		private const int MoveUpCode = 1;
		private const int MoveDownCode = 2;
		private const int MoveLeftCode = 3;
		private const int MoveRightCode = 4;

		#endregion

		#region Fields

		private readonly Arena arena;
		private readonly IMatchObserver observer;
		private readonly Dictionary<string, BomberProxy> bombersByNames = new();
		private readonly uint matchActionsNumber;
		private readonly uint bombDetonationRadius;
		private readonly uint bombTimeToDetonate;
		private readonly TimeSpan bomberActionTimeout;

		#endregion

		#region Ctor

		public Match(
			Arena arena,
			IMatchObserver observer,
			IReadOnlyList<IBomber> bombers,
			uint matchActionsNumber,
			uint bombDetonationRadius,
			uint bombTimeToDetonate,
			TimeSpan bomberActionTimeout)
		{
			this.arena = arena;
			this.observer = observer;
			this.matchActionsNumber = matchActionsNumber;
			this.bombDetonationRadius = bombDetonationRadius;
			this.bombTimeToDetonate = bombTimeToDetonate;
			this.bomberActionTimeout = bomberActionTimeout;

			bombersByNames = bombers.ToDictionary(bomber => bomber.Name, bomber => new BomberProxy(bomber));

			foreach (var bomber in bombers)
			{
				arena.RespawnBomber(bomber.Name, bombDetonationRadius, bombTimeToDetonate);
			}
		}

		#endregion		

		#region Public methods

		public MatchResult BombIt()
		{
			observer.StartMatch(bombDetonationRadius, bombTimeToDetonate, arena);

			try
			{
				foreach (var bomber in GetAllBombers())
				{
					try
					{
						bomber.SetRules(
						matchActionsNumber: (int)matchActionsNumber,
						detonationRadius: (int)bombDetonationRadius,
						timeToDetonate: (int)bombTimeToDetonate);
					}
					catch (Exception e)
					{
						throw new InvalidOperationException($"Unable to bomb! {bomber.Name} crashed while rules setting. {e.Message}");
					}
				}

				var deadBombers = new HashSet<string>();
				var matchResultBuilder = new MatchResult.Builder();

				for (var actionNumber = 1; actionNumber <= matchActionsNumber; actionNumber++)
				{
					GetArenaState(out var arenaMatrix, out var aliveBombersPoints);

					var matchRoundBuilder = new MatchRound.Builder($"#{actionNumber}");

					foreach (var bomberName in arena.AliveBombers)
					{
						var bomber = GetBomber(bomberName);

						var bombersMatrix = CreateBombersMatrix(aliveBombersPoints, bomber.Name);
						var availableMoves = arena
							.GetAvailableBomberMoves(bomber.Name)
							.Select(ConvertDirectionToCode)
							.Append(MoveNoneCode)
							.ToArray();

						try
						{
							var bomberActionCode = Runner.RunWithTimeout(() => bomber.Go(arenaMatrix, bombersMatrix, availableMoves), bomberActionTimeout);
							var bomberDesiredAction = ConvertCodeToAction(bomberActionCode);
							var bomberRealAction = ApplyBomberAction(bomber.Name, bomberDesiredAction);
							matchRoundBuilder.AddAction(bomber.Name, bomberDesiredAction, bomberRealAction);

							if (bomber.Suicide)
							{
								matchRoundBuilder.AddMistake(bomber.Name, new Exception("This is SUICIDE!"));
							}
						}
						catch (Exception ex)
						{
							bomber.AddPenalty();
							matchRoundBuilder.AddMistake(bomber.Name, ex);							
						}
					}								

					arena.Flush();

					foreach (var bomber in arena.DeadBombers)
					{
						if (deadBombers.Add(bomber))
						{
							matchRoundBuilder.AddKilledBomber(bomber);
							matchResultBuilder.AddBomberResult(bomber, false, actionNumber);
						}
					}

					var matchRound = matchRoundBuilder.Build();
					observer.AddRound(matchRound);

					var aliveBombers = arena.AliveBombers;
					switch (aliveBombers.Count)
					{
						case 0:
							return matchResultBuilder.Build();

						case 1:
							matchResultBuilder.AddBomberResult(aliveBombers[0], true, actionNumber);
							return matchResultBuilder.Build();
					}
				}

				foreach (var bomber in arena.AliveBombers)
				{
					matchResultBuilder.AddBomberResult(bomber, true, (int)matchActionsNumber);
				}
				return matchResultBuilder.Build();
			}
			finally
			{
				observer.EndMatch();
			}
		}

		#endregion

		#region Private methods

		private IReadOnlyCollection<BomberProxy> GetAllBombers()
		{
			return bombersByNames.Values;
		}

		private BomberProxy GetBomber(string bomberName)
		{
			if (!bombersByNames.TryGetValue(bomberName, out var bomber))
			{
				throw new InvalidOperationException($"Bomber '{bomberName}' not found");
			}
			return bomber;
		}

		private void GetArenaState(out int[,] matrix, out Dictionary<string, Point> aliveBombersPoints)
		{
			matrix = new int[arena.Fields.GetLength(0), arena.Fields.GetLength(1)];
			aliveBombersPoints = new Dictionary<string, Point>();

			for (var i = 0; i < matrix.GetLength(0); i++)
			{
				for (var j = 0; j < matrix.GetLength(1); j++)
				{
					var field = arena.Fields[i, j];
					if (field == null)
					{
						matrix[i, j] = FieldWallCode;
					}
					else
					{
						if (field.HasBomb)
						{
							matrix[i, j] = (int)field.Bomb.TimeToDetonate;
						}
						else
						{
							matrix[i, j] = FieldEmptyCode;
						}

						foreach (var bomber in field.Bombers)
						{
							if (bomber.IsAlive)
							{
								aliveBombersPoints.Add(bomber.Name, new Point { i = i, j = j });
							}
						}
					}
				}
			}
		}

		private int[,] CreateBombersMatrix(Dictionary<string, Point> bombersPoints, string mainBomberName)
		{
			var matrix = new int[bombersPoints.Count, 2];

			var mainBomberPoint = bombersPoints[mainBomberName];
			matrix[0, 0] = mainBomberPoint.i;
			matrix[0, 1] = mainBomberPoint.j;

			var rowIndex = 1;
			foreach (var bombersPoint in bombersPoints.OrderBy(point => point.Key))		// Сортировка для сохранения порядка на разных шагах
			{
				if (!string.Equals(bombersPoint.Key, mainBomberName))
				{
					matrix[rowIndex, 0] = bombersPoint.Value.i;
					matrix[rowIndex, 1] = bombersPoint.Value.j;
					rowIndex++;
				}
			}

			return matrix;
		}		

		private BomberAction ApplyBomberAction(string bomberName, BomberAction desiredAction)
		{
			var plantBomb = desiredAction.PlanBomb;
			Direction? movement = desiredAction.Movement;

			if (plantBomb)
			{
				plantBomb = arena.PlantBomb(bomberName);
			}

			if (movement.HasValue && !arena.MoveBomber(bomberName, movement.Value))
			{
				movement = null;
			}

			return new BomberAction(plantBomb, movement);
		}

		#endregion

		#region Helpers

		private static int ConvertDirectionToCode(Direction direction)
		{
			switch (direction)
			{
				case Direction.Up:
					return MoveUpCode;

				case Direction.Down:
					return MoveDownCode;

				case Direction.Left:
					return MoveLeftCode;

				case Direction.Right:
					return MoveRightCode;

				default:
					throw new ArgumentOutOfRangeException(nameof(direction));
			}
		}

		private static Direction? ConvertCodeToDirection(int directionCode)
		{
			switch (directionCode)
			{
				case MoveUpCode:
					return Direction.Up;

				case MoveDownCode:
					return Direction.Down;

				case MoveLeftCode:
					return Direction.Left;

				case MoveRightCode:
					return Direction.Right;

				case MoveNoneCode:
					return null;

				default:
					throw new ArgumentOutOfRangeException($"Unknown direction code '{directionCode}'");
			}
		}

		private static BomberAction ConvertCodeToAction(int actionCode)
		{
			var plantBomb = actionCode >= BompPlantingSign;
			if (plantBomb)
			{
				actionCode -= BompPlantingSign;
			}

			var movement = ConvertCodeToDirection(actionCode);

			return new BomberAction(plantBomb, movement);
		}

		#endregion

		#region Nested

		private struct Point
		{
			public int i;
			public int j;
		}

		private sealed class BomberProxy : IBomber
		{
			private readonly IBomber bomber;
			private int activePenalties;
			private int totalPenalties;

			public BomberProxy(IBomber bomber)
			{
				this.bomber = bomber;
			}

			public bool HasPenalties => activePenalties > 0;

			public bool Suicide => totalPenalties > 2;

			public string Name => bomber.Name;

			public void SetRules(int matchActionsNumber, int detonationRadius, int timeToDetonate)
			{
				bomber.SetRules(matchActionsNumber, detonationRadius, timeToDetonate);
			}

			public int Go(int[,] arena, int[,] bombers, int[] availableMoves)
			{
				if (Suicide)
				{
					// Самоубийство
					activePenalties--;
					return 10;
				}

				if (HasPenalties)
				{
					// Пропуск хода
					activePenalties--;
					return 0;
				}

				return bomber.Go(arena, bombers, availableMoves);
			}

			public void AddPenalty()
			{
				totalPenalties++;
				activePenalties = totalPenalties * 2;
			}
		}

		#endregion
	}
}