using BomberMatch;
using BomberMatch.Championships;
using BomberMatch.Observers;


var maps = new[]
{
	"Classic7x7B2",
	"Andromeda17x17B4"
};

var bombers = new[]
{
	new BomberDescriptor("Pt4k", ""),
	new BomberDescriptor("TimBomber", ""),
	new BomberDescriptor("Rabkahalla", ""),
	new BomberDescriptor("PythonBomber", "cobra.py")
};

var one = new OneOnOne(
	matchActionsNumber: 1000,
	bombDetonationRadius: 2,
	bombTimeToDetonate: 4,
	bomberActionTimeout: TimeSpan.FromSeconds(2),
	createMatchObserver: (index, mapName, bombersNames) => new MatchDummyObserver());

one.Bomb(maps, bombers);