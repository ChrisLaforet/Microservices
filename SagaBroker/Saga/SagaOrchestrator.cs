using SagaBroker.Exception;
using SagaBroker.StateMachine;
using SagaProxy.DBManagement;
using SagaProxy.QueueManagement;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace SagaBroker.Saga
{
	public class SagaOrchestrator
	{
		private readonly IDictionary<string, SagaStage> transitionStates = new Dictionary<string, SagaStage>();
		private bool isClosed = false;
		internal readonly SagaRemoteDriver remoteDriver;

		public SagaOrchestrator(string orchestratorName, IQueueDriver queueDriver, IDBDriver dbDriver)
		{
			Name = orchestratorName.ToUpper();
			DBDriver = dbDriver;
			remoteDriver = new SagaRemoteDriver(queueDriver);
		}

		public string Name { protected set; get; }

		internal IDBDriver DBDriver { private set; get; }

		internal ISagaStage FirstStage { set; get; }

		public IList<ISagaStage> Transitions
		{
			get
			{
				return transitionStates.Values.ToImmutableList<ISagaStage>();
			}
		}

		public void AddStageToSource(SagaStage parent, SagaStage stage)
		{
			AddStageToSource(parent?.Name, stage);
		}

		public void AddStageToSource(string parentStageName, SagaStage stage)
		{
			if (isClosed)
				throw new InvalidStageException("Attempt to add stage " + stage.Name + " to a closed orchestrator that has created state machines");

			// determine there are no cyclic dependencies
			foreach (var transition in Transitions)
			{
				if (transitionStates.Keys.Contains(transition.Name))
					throw new CyclicDependencyException(transition.Name + " in " + stage.Name);
			}

			if (parentStageName != null && FirstStage == null)
				throw new InvalidStageException("Attempt to add parented stage " + stage.Name + " without a first stage being defined");
			else if (parentStageName == null && FirstStage != null)
				throw new InvalidStageException("Attempt to add an additional first stage " + stage.Name + " when one is already defined");

			if (parentStageName == null)
			{
				FirstStage = stage;
				TrackTransitionStage(stage);
				return;
			}

			SagaStage state = transitionStates[parentStageName];
			if (state == null)
				throw new SagaTransitionException("Cannot locate parent state " + parentStageName + " for stage " + stage.Name);

			state.AddTransition(stage);
			TrackTransitionStage(stage);
		}

		private void TrackTransitionStage(SagaStage stage)
		{
			transitionStates.Add(stage.Name, stage);
		}

		public SagaStateMachine SagaStateMachine()
		{
			isClosed = true;
			return new SagaStateMachine(this);
		}
	}
}
