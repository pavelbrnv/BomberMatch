namespace BomberMatch.Bombers
{
	public class Psycho : IBomber
	{
		private readonly Random random = new Random();
		private int period = 7;
		private int timeToChoose = 7;

		public string Name { get; }

		public Psycho(string bomberIndex)
		{
			Name = $"Psycho{bomberIndex}";
		}		

		public void SetRules(int matchActionsNumber, int detonationRadius, int timeToDetonate)
		{
			period = detonationRadius + timeToDetonate;
			timeToChoose = period;
		}

		public int Go(int[,] arena, int[,] bombers, int[] availableMoves)
		{
			if (availableMoves.Length == 0)
			{
				return 0;
			}
			if (availableMoves.Length == 1)
			{
				return availableMoves[0];
			}

			var index = random.Next(0, availableMoves.Length);
			var actionCode = availableMoves[index];

			timeToChoose--;
			if (timeToChoose == 0)
			{
				var plantingDecision = random.Next(0, 100);
				if (plantingDecision < 40)
				{
					// Bomb this!
					actionCode += 10;
				}

				timeToChoose = period;
			}

			return actionCode;
		}
	}
}
