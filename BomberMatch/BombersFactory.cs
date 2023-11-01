namespace BomberMatch
{
	public static class BombersFactory
	{
		private static readonly Dictionary<string, IBomberFactory> factoriesMap;

		static BombersFactory()
		{
			factoriesMap = BombersRegister.Factories.ToDictionary(factory => factory.Name, factory => factory);
		}

		public static IReadOnlyCollection<string> GetBombersNames()
		{
			return factoriesMap.Keys;
		}

		public static int GetBombersCount()
		{
			return factoriesMap.Count;
		}

		public static IBomber CreateBomber(string bomberName, string parameter)
		{
			return factoriesMap[bomberName].Create(parameter);
		}

		public static IBomber CreateBomber(BomberDescriptor descriptor)
		{
			return CreateBomber(descriptor.BomberName, descriptor.Parameter);
		}
	}

	public sealed class BomberDescriptor
	{
		public string BomberName { get; }		

		public string Parameter { get; }

		public BomberDescriptor(string bomberName, string parameter)
		{
			BomberName = bomberName;
			Parameter = parameter;
		}
	}

	public interface IBomberFactory
	{
		string Name { get; }

		IBomber Create(string parameter);
	}

	public sealed class BomberFactory<T> : IBomberFactory
		where T : IBomber
	{
		private readonly Func<string, T> create;

		public BomberFactory(Func<string, T> create)
		{
			this.create = create;
		}

		public string Name => typeof(T).Name;

		public IBomber Create(string parameter)
		{
			return create(parameter);
		}
	}
}
