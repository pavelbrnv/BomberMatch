namespace BomberMatch.Bombers;

public class AtdBomber : IBomber
{
	private int detonationRadius;
	private int timeToDetonate;
	private Random r = new Random(1669);

	#region IBomber

	public string Name => "Tim_Arkhipov";

	public void SetRules(int maxActionsNumber, int detonationRadius, int timeToDetonate)
	{
		this.detonationRadius = detonationRadius;
		this.timeToDetonate = timeToDetonate;
	}

	public int Go(int[,] arena, int[,] bombers, int[] availableActions)
	{
		try
		{
			var myPosX = bombers[0, 0];
			var myPosY = bombers[0, 1];
			var p = new CurrentStepProcessor(detonationRadius, timeToDetonate, arena, bombers);

			//рядом бомба - просто убегаем
			if (p.IfBombIsNear(myPosX, myPosY))
			{
				foreach (var action in availableActions)
				{
					var newPos = MakeStep(myPosX, myPosY, action);

					if (!p.IfBombIsNear(newPos.Item1, newPos.Item2))
						return action;
				}
				// а если не нашли укромное место, то забиваем и стоим.
				return 0;
			}

			//иначе - идём в сторону соперника
			//if (p.IfBomberIsNear(myPosX, myPosY))

			var z = GenerateSafeStep(p, myPosX, myPosY, availableActions);
			return availableActions.Contains(z) ? z + 10 : 0;//проверка на всякий случай
		}
		catch
		{
			//var myActions = new int[availableActions.Length + 1];
			//myActions[r.Next()]
			return availableActions.Length > 0 ? availableActions[r.Next(availableActions.Length)] : 0;
		}
	}

	private int GenerateSafeStep(CurrentStepProcessor p, int myPosX, int myPosY, int[] availableActions)
	{
		foreach (var action in availableActions)
		{
			var newPos = MakeStep(myPosX, myPosY, action);

			if (!p.IfBombIsNear(newPos.Item1, newPos.Item2))
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
	/*
	private int GenerateStepToBomber(CurrentStepProcessor p, int myPosX, int myPosY)
	{
		if (p.IfBomberOnTop(myPosX, myPosY) && )
	}
	*/
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
			this.opponentPos = new Tuple<int, int>(bombers[1, 0], bombers[1, 1]);
		}

		public bool IfBombIsNear(int x, int y)
		{
			try
			{
				return IfBombOnLeft(x, y) ||
					   IfBombOnRight(x, y) ||
					   IfBombOnTop(x, y) ||
					   IfBombOnBottom(x, y);
			}
			catch
			{
				return false;
			}
		}

		#region bomber

		public bool IfBomberIsNear(int x, int y)
		{
			//проблема, что не учитываются стены
			var dist = Math.Abs(x - opponentPos.Item1) + Math.Abs(y - opponentPos.Item2);
			return dist <= detonationRadius;
		}

		public bool IfBomberOnLeft(int x, int y)
		{
			return opponentPos.Item1 < x;
		}

		public bool IfBomberOnRight(int x, int y)
		{
			return opponentPos.Item1 > x;
		}
		public bool IfBomberOnTop(int x, int y)
		{
			return opponentPos.Item2 < y;
		}
		public bool IfBomberOnBottom(int x, int y)
		{
			return opponentPos.Item2 > y;
		}

		#endregion

		public bool IfBombOnLeft(int x, int y)
		{
			int fromX = Math.Max(0, x - detonationRadius);
			for (int i = x; i >= fromX; i--)
			{
				switch (arena[i, y])
				{
					case -1: return false;
					case 0: break;
					default: return true;
				}
			}
			return false;
		}

		public bool IfBombOnRight(int x, int y)
		{
			int toX = Math.Min(x + detonationRadius, arena.GetLength(0));
			for (int i = x; i <= toX; i++)
			{
				switch (arena[i, y])
				{
					case -1: return false;
					case 0: break;
					default: return true;
				}
			}
			return false;
		}
		public bool IfBombOnTop(int x, int y)
		{
			int fromY = Math.Max(0, y - detonationRadius);
			for (int j = y; j >= fromY; j--)
			{
				switch (arena[x, j])
				{
					case -1: return false;
					case 0: break;
					default: return true;
				}
			}
			return false;
		}
		public bool IfBombOnBottom(int x, int y)
		{
			int toY = Math.Min(y + detonationRadius, arena.GetLength(1));
			for (int j = y; j <= toY; j++)
			{
				switch (arena[x, j])
				{
					case -1: return false;
					case 0: break;
					default: return true;
				}
			}
			return false;
		}

		public bool IsWall(int x, int y)
		{
			if (x < 0 || y < 0 || x >= arena.GetLength(0) || y >= arena.GetLength(1))
				return true;

			return arena[x, y] == -1;
		}
	}

}