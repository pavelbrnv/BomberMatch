using System;

namespace BomberMatch.Bombers
{
	public sealed class Walker : IBomber
	{
		public string Name { get; }

        public Walker(string bomberIndex)
		{
			Name = $"Walker{bomberIndex}";
		}

		public void SetRules(int matchActionsNumber, int detonationRadius, int timeToDetonate)
		{
		}

		public int Go(int[,] arena, int[,] bombers, int[] availableActions)
		{
			if (availableActions.Length > 0)
			{
				return availableActions[0];
			}

			return 0;
		}
	}
}
