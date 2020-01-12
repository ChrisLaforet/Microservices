using System;
using System.Collections.Generic;
using SagaBroker.StateMachine;
using SagaProxy.DBManagement;

namespace SagaBroker.Saga
{
	[Flags]
	public enum RewindStrategy
	{
		ContinueOnError = 0,
		FailOnError = 1,

	};

	public interface ISagaOrchestrator
	{
		string Name { get; }

		IList<ISagaStage> Transitions { get; }

		void InsertStage(ISagaStage stage);

		void Orchestrate(IOperationData operationData);

		ISagaRemoteDriver RemoteQueueDriver { get; }

		IDBDriver DBDriver { get; }

		RewindStrategy RewindStrategyOptions { get; }

		ISagaStage RootStage { get; }

		ISagaStage GetStageByName(string stageName);
	}
}