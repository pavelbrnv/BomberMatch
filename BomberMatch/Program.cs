using BomberMatch;
using BomberMatch.Bombers;

// 0 - пустота
// 1 - поле без респауна
// 2 - поле с респауном
var map5 = new int[5, 5]
{
	{ 2, 1, 1, 1, 1 },
	{ 1, 0, 1, 0, 1 },
	{ 1, 1, 1, 1, 1 },
	{ 1, 0, 1, 0, 1 },
	{ 1, 1, 1, 1, 2 }
};
var map7 = new int[7, 7]
{
	{ 2, 1, 1, 1, 1, 1, 1 },
	{ 1, 0, 1, 0, 1, 0, 1 },
	{ 1, 1, 1, 1, 1, 1, 1 },
	{ 1, 0, 1, 0, 1, 0, 1 },
	{ 1, 1, 1, 1, 1, 1, 1 },
	{ 1, 0, 1, 0, 1, 0, 1 },
	{ 1, 1, 1, 1, 1, 1, 2 }
};

var arena = Arena.Build(map7);

// Walker("Free")
// Kamikaze("Angry")
// AtdBomber()
// TimBomber()
// Killer()
// Psycho("Wow")

var bomber1 = new Psycho("Foo");
var bomber2 = new Psycho("Bar");

var match = new Match(
	arena: arena,
	bombers: new IBomber[] { new Voyeur(bomber1), bomber2 },
	matchActionsNumber: 1000,
	bombDetonationRadius: 2,
	bombTimeToDetonate: 4);

var result = match.BombIt();

Console.Write(result);