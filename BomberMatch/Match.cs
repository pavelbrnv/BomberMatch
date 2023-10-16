namespace BomberMatch
{
	public sealed class Match
	{
		#region Consts

		private const int BompPlantingSign = 10;
		private static readonly TimeSpan BomberActionTimeout = TimeSpan.FromSeconds(3);

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
		private readonly Dictionary<string, IBomber> bombersByNames = new();
		private readonly uint matchActionsNumber;
		private readonly uint bombDetonationRadius;
		private readonly uint bombTimeToDetonate;

		#endregion

		#region Ctor

		public Match(
			Arena arena,
			IReadOnlyList<IBomber> bombers,
			uint matchActionsNumber,
			uint bombDetonationRadius,
			uint bombTimeToDetonate)
		{
			this.arena = arena;
			this.matchActionsNumber = matchActionsNumber;
			this.bombDetonationRadius = bombDetonationRadius;
			this.bombTimeToDetonate = bombTimeToDetonate;

			bombersByNames = bombers.ToDictionary(bomber => bomber.Name, bomber => bomber);

			foreach (var bomber in bombers)
			{
				arena.RespawnBomber(bomber.Name, bombDetonationRadius, bombTimeToDetonate);
			}
		}

		#endregion

		#region Public methods

		public string BombIt()
		{
			foreach (var bomber in bombersByNames.Values)
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

			for (var actionNumber = 0; actionNumber < matchActionsNumber; actionNumber++)
			{
				GetArenaState(out var arenaMatrix, out var aliveBombersPoints);

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
						var bomberActionCode = Runner.RunWithTimeout(() => bomber.Go(arenaMatrix, bombersMatrix, availableMoves), BomberActionTimeout);

						if (bomberActionCode >= BompPlantingSign)
						{
							arena.PlantBomb(bomber.Name);
							bomberActionCode -= BompPlantingSign;
						}

						if (TryConvertCodeToDirection(bomberActionCode, out var direction))
						{
							arena.MoveBomber(bomber.Name, direction);
						}
					}
					catch (Exception e)
					{
						return $"{bomber.Name}: {e.Message}";
					}					
				}

				arena.Flush();

				var aliveBombers = arena.AliveBombers;
				switch (aliveBombers.Count)
				{
					case 0:
						return $"Draw! No one survived [action #{actionNumber}]";
					case 1:
						return $"The winner is ... {aliveBombers[0]} [action #{actionNumber}]";
				}
			}

			return "Draw! Timeout";
		}

		#endregion

		#region Private methods

		private IBomber GetBomber(string bomberName)
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

		private static bool TryConvertCodeToDirection(int directionCode, out Direction direction)
		{
			switch (directionCode)
			{
				case MoveUpCode:
					direction = Direction.Up;
					return true;

				case MoveDownCode:
					direction = Direction.Down;
					return true;

				case MoveLeftCode:
					direction = Direction.Left;
					return true;

				case MoveRightCode:
					direction = Direction.Right;
					return true;

				default:
					direction = default(Direction);
					return false;
			}
		}

		#endregion

		#region Nested

		private struct Point
		{
			public int i;
			public int j;
		}

		#endregion
	}
}