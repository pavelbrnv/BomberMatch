using BomberMatch.Bombers;
using BomberMatch.Bombers.Assembly;
using BomberMatch.Bombers.C;
using BomberMatch.Bombers.Js;
using BomberMatch.Bombers.Python;

namespace BomberMatch
{
	public static class BombersRegister
	{
		public static readonly IReadOnlyList<IBomberFactory> Factories = new IBomberFactory[]
		{
			new BomberFactory<Psycho>(parameter => new Psycho(parameter)),
			new BomberFactory<Kamikaze>(parameter => new Kamikaze(parameter)),
			new BomberFactory<Walker>(parameter => new Walker(parameter)),

			new BomberFactory<AssemblyBomber>(parameter => new AssemblyBomber(parameter)),
			new BomberFactory<CBomber>(parameter => new CBomber(parameter)),
			new BomberFactory<PythonBomber>(parameter => new PythonBomber(parameter)),
			new BomberFactory<JsBomber>(parameter => new JsBomber(parameter))
		};
	}
}
