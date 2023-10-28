namespace BomberMatch.Bombers.Python
{
	public sealed class PythonBomber : IBomber
	{
		private readonly dynamic getName;
		private readonly dynamic setRules;
		private readonly dynamic go;

		public PythonBomber(string scriptName)
		{			
			var engine = IronPython.Hosting.Python.CreateEngine();
			var scope = engine.CreateScope();
			engine.ExecuteFile(scriptName, scope);
			var bomberType = scope.GetVariable("Bomber");
			var bomber = engine.Operations.CreateInstance(bomberType);

			getName = engine.Operations.GetMember<Func<string>>(bomber, "get_name");
			setRules = engine.Operations.GetMember<Action<int, int, int>>(bomber, "set_rules");
			go = engine.Operations.GetMember<Func<int[][], int[][], int[], int>>(bomber, "go");
		}

		public string Name => getName();
		
		public void SetRules(int matchActionsNumber, int detonationRadius, int timeToDetonate)
		{
			setRules(matchActionsNumber, detonationRadius, timeToDetonate);
		}

		public int Go(int[,] arena, int[,] bombers, int[] availableMoves)
		{
			return go(ConvertArray(arena), ConvertArray(bombers), availableMoves);
		}

		private static int[][] ConvertArray(int[,] items)
		{
			var rows = items.GetLength(0);
			var columns = items.GetLength(1);

			var result = new int[rows][];

			for (var i = 0; i < columns; i++)
			{
				var row = new int[columns];
				for (var j = 0; j < columns; j++)
				{
					row[j] = items[i, j];
				}
				result[i] = row;
			}

			return result;
		}
	}
}
