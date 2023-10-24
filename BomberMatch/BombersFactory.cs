using BomberMatch.Bombers;
using BomberMatch.Bombers.Razbomber;

namespace BomberMatch
{
	public static class BombersFactory
	{
		public delegate IBomber BomberCreator();

		public static readonly IReadOnlyList<BomberCreator> Creators = new BomberCreator[]
		{
			() => new Pt4k(),
			() => new Razbomber(),
			() => new TimBomber(),
			() => new Killer(),
			() => new Psycho("Redneck"),
			() => new Rabkahalla(),
			() => new AssemblyLoader("BomberRadius.dll")
		};
	}
}
