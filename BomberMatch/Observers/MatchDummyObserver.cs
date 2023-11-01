namespace BomberMatch.Observers
{
    public sealed class MatchDummyObserver : IMatchObserver
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

        public void Dispose()
        {
        }
    }
}
