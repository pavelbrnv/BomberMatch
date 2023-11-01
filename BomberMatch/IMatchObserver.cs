namespace BomberMatch
{
	public interface IMatchObserver : IDisposable
	{
		void StartMatch(uint bombDetonationRadius, uint bombTimeToDetonate, Arena arena);

		void AddRound(MatchRound round);

		void EndMatch();
	}

	public delegate IMatchObserver MatchObserverCreator(int index, string mapName, params string[] bombersNames);

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
		public MatchRound(string description, IReadOnlyList<BomberValidatedAction> validatedActions, IReadOnlyList<BomberMistake> mistakes, IReadOnlyList<string> killedBombers)
		{
			Description = description;
			ValidatedActions = validatedActions;
			Mistakes = mistakes;
			KilledBombers = killedBombers;
		}

		public string Description { get; }

		public IReadOnlyList<BomberValidatedAction> ValidatedActions { get; }		

		public IReadOnlyList<BomberMistake> Mistakes { get; }

		public IReadOnlyList<string> KilledBombers { get; }

		#region Nested

		public sealed class Builder
		{
			private readonly string description;

			private readonly List<BomberValidatedAction> validatedActions = new();
			private readonly List<BomberMistake> mistakes = new();
			private readonly List<string> killedBombers = new();


			public Builder(string description)
			{
				this.description = description;
			}

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

			public void AddKilledBomber(string bomberName)
			{
				killedBombers.Add(bomberName);
			}

			public MatchRound Build()
			{
				return new MatchRound(description, validatedActions, mistakes, killedBombers);
			}
		}

		#endregion
	}
}
