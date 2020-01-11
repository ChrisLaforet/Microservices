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
		public enum RewindStrategy
		{
			ContinueOnError = 0,
			FailOnError = 1,

		};

		private readonly IDictionary<string, ISagaStage> stageLookup = new Dictionary<string, ISagaStage>();
		private bool isClosed = false;

		public SagaOrchestrator(string orchestratorName, IQueueDriver queueDriver, IDBDriver dbDriver, 
			ISagaStage rootStage, RewindStrategy rewindStrategyOptions = RewindStrategy.ContinueOnError)
		{
			Name = orchestratorName.ToUpper();
			DBDriver = dbDriver;
			RemoteQueueDriver = new SagaRemoteDriver(queueDriver);
			RootStage = rootStage;
			RewindStrategyOptions = rewindStrategyOptions;
		}

		public string Name { protected set; get; }

		internal SagaRemoteDriver RemoteQueueDriver { private set; get; }

		internal IDBDriver DBDriver { private set; get; }

		internal RewindStrategy RewindStrategyOptions { private set; get; }

		internal ISagaStage RootStage { set; get; }

		public void Orchestrate(IOperationData operationData)
		{
			isClosed = true;
			SagaStateMachine machine = new SagaStateMachine(this);
			machine.Run(operationData);
		}

		public IList<ISagaStage> Transitions
		{
			get
			{
				return stageLookup.Values.ToImmutableList<ISagaStage>();
			}
		}

		public void InsertStage(ISagaStage stage)
		{
			if (isClosed)
				throw new InvalidStageException("Attempt to add stage " + stage.Name + " to a closed orchestrator that has created state machines");
			AddStageToLookup(stage);
		}

		internal ISagaStage GetStageByName(string stageName)
		{
			return stageLookup[stageName];
		}

		private void AddStageToLookup(ISagaStage stage)
		{
			stageLookup.Add(stage.Name, stage);
		}
	}
}
