using System.Reflection;

namespace BomberMatch
{
	public static class ArenasFactory
	{
		public static IEnumerable<string> GetMapsNames()
		{
			var fields = typeof(Maps).GetFields(BindingFlags.Public | BindingFlags.Static);
			foreach (var field in fields)
			{
				if (field.FieldType == typeof(int[,]))
				{
					yield return field.Name;
				}
			}
		}

		public static int[,] GetMap(string mapName)
		{
			var field = typeof(Maps).GetField(mapName, BindingFlags.Public | BindingFlags.Static);
			return (int[,])field.GetValue(null);
		}

		public static Arena CreateArena(string mapName)
		{
			var map = GetMap(mapName);
			return Arena.Build(map);
		}
	}
}
