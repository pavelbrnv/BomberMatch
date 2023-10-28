namespace BomberMatch
{
	public sealed class DummyObserver : IMatchObserver
	{
		public void StartMatch(uint bombDetonationRadius, uint bombTimeToDetonate, Arena arena)
		{
		}

		public void AddRound(MatchRound round)
		{
		}

		public void EndMatch()
		{
		}		
	}
}
