using System.Drawing;

namespace BomberMatch
{
	public sealed class Arena
	{
		#region Fields

		private readonly Field?[,] fields;
		private readonly Dictionary<string, Man> bombers = new();

		#endregion

		#region Ctor

		public Arena(Field?[,] fields)
		{
			this.fields = fields;

			InitArenaFields();
		}

		/// <summary>
		/// 0 - пустота
		/// 1 - поле без респауна
		/// 2 - поле с респауном
		/// </summary>
		public static Arena Build(int[,] codes)
		{
			var fields = new Field?[codes.GetLength(0), codes.GetLength(1)]; ;

			for (var i = 0; i < codes.GetLength(0); i++)
			{
				for (var j = 0; j < codes.GetLength(1); j++)
				{
					switch (codes[i, j])
					{
						case 0:
							fields[i, j] = null;
							break;

						case 1:
							fields[i, j] = new Field();
							break;

						case 2:
							fields[i, j] = new Field(true);
							break;

						default:
							throw new ArgumentException($"Unknown code for arena building '{codes[i, j]}'");
					}
				}
			}

			return new Arena(fields);
		}

		#endregion

		#region Properties

		public Field?[,] Fields => fields;

		public IReadOnlyList<string> AliveBombers =>
			bombers.Values
				.Where(bomber => bomber.IsAlive)
				.Select(bomber => bomber.Name)
				.ToArray();
		
		#endregion

		#region Public methods

		public void RespawnBomber(string bomberName, uint bombDetonationRadius, uint bombTimeToDetonate)
		{
			var field = GetAllFields().FirstOrDefault(field => field.AllowRespawn && field.Bombers.Count == 0);
			if (field == null)
			{
				throw new InvalidOperationException("Empty field for respawn not found");
			}

			var bomber = new Man(bomberName, bombDetonationRadius, bombTimeToDetonate, field);
			bombers.Add(bomber.Name, bomber);

			field.AddBomber(bomber);
		}

		public IReadOnlyList<Direction> GetAvailableBomberMoves(string bomberName)
		{
			var bomber = GetBomber(bomberName);

			ThrowIfBomberIsDead(bomber);

			var currentField = bomber.CurrentField;

			var availableMoves = new List<Direction>();

			if (currentField.UpField is { HasBomb: false })
			{
				availableMoves.Add(Direction.Up);
			}
			if (currentField.DownField is { HasBomb: false })
			{
				availableMoves.Add(Direction.Down);
			}
			if (currentField.LeftField is { HasBomb: false })
			{
				availableMoves.Add(Direction.Left);
			}
			if (currentField.RightField is { HasBomb: false })
			{
				availableMoves.Add(Direction.Right);
			}

			return availableMoves;
		}

		public bool MoveBomber(string bomberName, Direction direction)
		{
			var bomber = GetBomber(bomberName);

			ThrowIfBomberIsDead(bomber);

			var currentBomberField = bomber.CurrentField;

			Field? newBomberField = null;
			switch (direction)
			{
				case Direction.Up:
					newBomberField = currentBomberField.UpField;
					break;

				case Direction.Down:
					newBomberField = currentBomberField.DownField;
					break;

				case Direction.Left:
					newBomberField = currentBomberField.LeftField;
					break;

				case Direction.Right:
					newBomberField = currentBomberField.RightField;
					break;
			}
			if (newBomberField == null)
			{
				return false;
			}

			currentBomberField.RemoveBomber(bomber);
			newBomberField.AddBomber(bomber);

			bomber.MoveToField(newBomberField);
			return true;
		}

		public bool PlantBomb(string bomberName)
		{
			var bomber = GetBomber(bomberName);

			ThrowIfBomberIsDead(bomber);

			var bomb = bomber.GetBomb();
			return bomber.CurrentField.PlantBomb(bomb);
		}

		public void Flush()
		{
			foreach (var field in fields)
			{
				field?.Tick();
			}
		}

		#endregion

		#region Helpers

		private void InitArenaFields()
		{
			for (var i = 0; i < fields.GetLength(0); i++)
			{
				for (var j = 0; j < fields.GetLength(1); j++)
				{
					var field = fields[i, j];
					if (field != null)
					{
						if (i > 0 && fields[i - 1, j] != null)
						{
							field.UpField = fields[i - 1, j];
						}
						if (i < fields.GetLength(0) - 1 && fields[i + 1, j] != null)
						{
							field.DownField = fields[i + 1, j];
						}
						if (j > 0 && fields[i, j - 1] != null)
						{
							field.LeftField = fields[i, j - 1];
						}
						if (j < fields.GetLength(1) - 1 && fields[i, j + 1] != null)
						{
							field.RightField = fields[i, j + 1];
						}
					}
				}
			}
		}

		private IEnumerable<Field> GetAllFields()
		{
			for (var i = 0; i < fields.GetLength(0); i++)
			{
				for (var j = 0; j < fields.GetLength(1); j++)
				{
					var field = fields[i, j];
					if (field != null)
					{
						yield return field;
					}
				}
			}
		}

		private Man GetBomber(string bomberName)
		{
			if (!bombers.TryGetValue(bomberName, out var bomber))
			{
				throw new InvalidOperationException($"Bomber '{bomberName}' not found");
			}
			return bomber;
		}

		private void ThrowIfBomberIsDead(Man bomber)
		{
			if (!bomber.IsAlive)
			{
				throw new InvalidOperationException($"Bomber {bomber.Name} is dead");
			}
		}

		#endregion
	}
}
