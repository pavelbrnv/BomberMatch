namespace BomberMatch
{
	public interface IMatchObserver
	{
		void StartMatch(uint bombDetonationRadius, uint bombTimeToDetonate, Arena arena);

		void AddRound(MatchRound round);

		void EndMatch();
	}

	public sealed class BomberValidatedAction
	{
		public BomberValidatedAction(string bomberName, BomberAction desiredAction, BomberAction realAction)
		{
			BomberName = bomberName;
			DesiredAction = desiredAction;
			RealAction = realAction;
		}

		public string BomberName { get; }		

		public BomberAction DesiredAction { get; }

		public BomberAction RealAction { get; }
	}

	public sealed class BomberMistake
	{
		public BomberMistake(string bomberName, Exception mistake)
		{
			BomberName = bomberName;
			Mistake = mistake;
		}

		public string BomberName { get; }		

		public Exception Mistake { get; }
	}

	public sealed class MatchRound
	{
		public MatchRound(IReadOnlyList<BomberValidatedAction> validatedActions, IReadOnlyList<BomberMistake> mistakes)
		{
			ValidatedActions = validatedActions;
			Mistakes = mistakes;
		}

		public IReadOnlyList<BomberValidatedAction> ValidatedActions { get; }		

		public IReadOnlyList<BomberMistake> Mistakes { get; }

		#region Nested

		public sealed class Builder
		{
			private readonly List<BomberValidatedAction> validatedActions = new();
			private readonly List<BomberMistake> mistakes = new();

			public void AddAction(BomberValidatedAction action)
			{
				validatedActions.Add(action);
			}

			public void AddAction(string bomberName, BomberAction desiredAction, BomberAction realAction)
			{
				AddAction(new BomberValidatedAction(bomberName, desiredAction, realAction));
			}

			public void AddMistake(BomberMistake mistake)
			{
				mistakes.Add(mistake);
			}

			public void AddMistake(string bomberName, Exception mistake)
			{
				AddMistake(new BomberMistake(bomberName, mistake));
			}

			public MatchRound Build()
			{
				return new MatchRound(validatedActions, mistakes);
			}
		}

		#endregion
	}
}
