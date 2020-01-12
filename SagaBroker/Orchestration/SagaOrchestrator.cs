using SagaBroker.Exception;
using SagaBroker.Saga;
using SagaBroker.StateMachine;
using SagaProxy.DBManagement;
using SagaProxy.QueueManagement;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace SagaBroker.Orchestration
{
	public class SagaOrchestrator : ISagaOrchestrator
	{
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

		public ISagaRemoteDriver RemoteQueueDriver { private set; get; }

		public IDBDriver DBDriver { private set; get; }

		public RewindStrategy RewindStrategyOptions { private set; get; }

		public ISagaStage RootStage { private set; get; }

		public void ValidateStages()
		{
			throw new System.NotImplementedException();
		}

		public void Orchestrate(IOperationData operationData)
		{
			isClosed = true;

			if (RootStage == null)
				throw new SagaTransitionException("Attempt to orchestrate a workflow without a starting root stage");
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

		public ISagaStage GetStageByName(string stageName)
		{
			stageName = stageName.ToUpper();
			lock (stageLookup)
			{
				if (stageLookup.ContainsKey(stageName))
					return stageLookup[stageName];
			}
			return null;
		}

		private void AddStageToLookup(ISagaStage stage)
		{
			lock (stageLookup)
			{
				if (stageLookup.ContainsKey(stage.Name))
					throw new CyclicDependencyException("Stage " + stage.Name + " already exists in this orchestrator");
				stageLookup.Add(stage.Name, stage);
			}
			if (RootStage == null)
				RootStage = stage;
		}
	}
}
