using BomberMatch.Bombers;
using BomberMatch.Bombers.Assembly;
using BomberMatch.Bombers.Js;
using BomberMatch.Bombers.Python;
using BomberMatch.Bombers.Razbomber;

namespace BomberMatch
{
	public static class BombersRegister
	{
		public static readonly IReadOnlyList<IBomberFactory> Factories = new IBomberFactory[]
		{
			new BomberFactory<Pt4k>(parameter => new Pt4k()),
			new BomberFactory<TimBomber>(parameter => new TimBomber()),
			new BomberFactory<Rabkahalla>(parameter => new Rabkahalla()),
			new BomberFactory<Razbomber>(parameter => new Razbomber()),
			new BomberFactory<Killer>(parameter => new Killer()),

			new BomberFactory<Psycho>(parameter => new Psycho(parameter)),
			new BomberFactory<Kamikaze>(parameter => new Kamikaze(parameter)),
			new BomberFactory<Walker>(parameter => new Walker(parameter)),

			new BomberFactory<AssemblyBomber>(parameter => new AssemblyBomber(parameter)),
			new BomberFactory<PythonBomber>(parameter => new PythonBomber(parameter)),
			new BomberFactory<JsBomber>(parameter => new JsBomber(parameter))
		};
	}
}
