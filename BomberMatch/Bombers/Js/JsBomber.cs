using Jint;

namespace BomberMatch.Bombers.Js
{
    public sealed class JsBomber : IBomber
    {
        private readonly Engine engine;
        
        public JsBomber(string scriptName)
        {
            engine = new Engine();
            engine.Execute(File.ReadAllText(scriptName));
        }
        
        public string Name => GetName();

        public int Go(int[,] arena, int[,] bombers, int[] availableMoves)
        {
            return (int)engine.SetValue("arena", ConvertArray(arena))
                .SetValue("bombers", ConvertArray(bombers))
                .SetValue("availableMove", availableMoves)
                .Evaluate("go(arena, bombers, availableMoves)")
                .AsNumber();
        }

        public void SetRules(int matchActionsNumber, int detonationRadius, int timeToDetonate)
        {
            engine.SetValue("matchActionsNumber", matchActionsNumber)
                .SetValue("detonationRadius", detonationRadius)
                .SetValue("timeToDetonate", timeToDetonate)
                .Evaluate("setRules(matchActionsNumber, detonationRadius, timeToDetonate)");
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
        
        private string GetName()
        {
            return engine.Evaluate("getName()").AsString();
        }
    }
}