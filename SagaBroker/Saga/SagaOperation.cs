using SagaBroker.StateMachine;

namespace SagaBroker.Saga
{
	public abstract class SagaOperation : IOperationData
	{
		public int ExpirationMsec { private set;  get; }
		public string Name { private set;  get; }

		public abstract void OperationCallback(SagaOrchestrator orchestrator);
		public abstract void CompensationCallback(SagaOrchestrator orchestrator);
	}
}
