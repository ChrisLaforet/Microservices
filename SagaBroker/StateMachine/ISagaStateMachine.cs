
namespace SagaBroker.StateMachine
	{
	public interface ISagaStateMachine
		{
		StepState ExecuteCompensatingTransaction(IOperationData operationData);
		string ExecuteTransaction(IOperationData operationData);
		}
	}