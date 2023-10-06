namespace BomberMatch.Bombers
{
    public sealed class Kamikaze : IBomber
    {
        public string Name { get; }

        public Kamikaze(string bomberIndex)
        {
			Name = $"Kamikaze{bomberIndex}";
		}

        public void SetRules(int matchActionsNumber, int detonationRadius, int timeToDetonate)
        {
            //Banzai!
        }

        public int Go(int[,] arena, int[,] bombers, int[] availableMoves)
        {
            var random = new Random();
            var index = random.Next(0, availableMoves.Length);
            return availableMoves[index] + 10;
        }
    }
}
