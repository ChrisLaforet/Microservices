using SagaBroker.Exception;
using SagaBroker.StateMachine;
using SagaProxy.DBManagement;
using SagaProxy.QueueManagement;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace SagaBroker.Orchestration
{
	public class SagaOrchestrator
	{
		[Flags]
		public enum RewindStrategy : int
		{
			ContinueOnError = 0,
			FailOnError = 1,

		};

		private readonly IDictionary<string, ISagaStage> transitionStates = new Dictionary<string, ISagaStage>();
		private bool isClosed = false;

		public SagaOrchestrator(string orchestratorName, IQueueDriver queueDriver, IDBDriver dbDriver, 
			RewindStrategy rewindStrategyOptions = RewindStrategy.ContinueOnError)
		{
			Name = orchestratorName.ToUpper();
			DBDriver = dbDriver;
			RemoteQueueDriver = new SagaRemoteDriver(queueDriver);
			RewindStrategyOptions = rewindStrategyOptions;
		}

		public string Name { protected set; get; }

		internal SagaRemoteDriver RemoteQueueDriver { private set; get; }

		internal IDBDriver DBDriver { private set; get; }

		internal RewindStrategy RewindStrategyOptions { private set; get; }

		internal ISagaStage FirstStage { set; get; }

		public void Orchestrate(IOperationData operationData)
		{
			SagaStateMachine machine = new SagaStateMachine(this);
			machine.Run(operationData);
		}

		public IList<ISagaStage> Transitions
		{
			get
			{
				return transitionStates.Values.ToImmutableList<ISagaStage>();
			}
		}

		public void AddStageToSource(ISagaStage parent, ISagaStage stage)
		{
			AddStageToSource(parent?.Name, stage);
		}

		public void AddStageToSource(string parentStageName, ISagaStage stage)
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

			ISagaStage state = transitionStates[parentStageName];
			if (state == null)
				throw new SagaTransitionException("Cannot locate parent state " + parentStageName + " for stage " + stage.Name);

state.AddTransition(stage);
			TrackTransitionStage(stage);
		}

		internal ISagaStage GetStageByName(string stageName)
		{

		}

private void TrackTransitionStage(ISagaStage stage)
{
	transitionStates.Add(stage.Name, stage);
}
	}
}
