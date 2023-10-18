namespace BomberMatch
{
	public sealed class BomberAction
	{
		public BomberAction(bool planBomb, Direction? movement)
		{
			PlanBomb = planBomb;
			Movement = movement;
		}

		public bool PlanBomb { get; }

		public Direction? Movement { get; }
	}
}
