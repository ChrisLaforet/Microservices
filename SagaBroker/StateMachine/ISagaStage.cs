using SagaBroker.Orchestration;
using SagaBroker.Saga;


namespace SagaBroker.StateMachine
{
	public enum StepState
	{
		STEP_SUCCESS,
		STEP_FAILURE,
		STEP_EXIT,
		STEP_NODELEGATE
	}

	public delegate StepState SagaOperation(ISagaRemoteDriver sagaRemoteDriver,IOperationData operationData);

	public interface ISagaStage
	{
		string Name { get; }

		string TransactionTransitionOnSuccess { get; }

		string TransactionTransitionOnFailure { get; }

		string TransactionTransitionOnExit { get; }

		string ExecuteTransaction(ISagaRemoteDriver sagaRemoteDriver,IOperationData operationData);

		StepState ExecuteCompensatingTransaction(ISagaRemoteDriver sagaRemoteDriver,IOperationData operationData);
	}
}
