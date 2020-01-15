
namespace SagaBroker.StateMachine
	{
	public interface ISagaStateMachine
		{
		int StagesExecuted { get; }
		void Run(IOperationData operationData);
		StepState ExecuteCompensatingTransaction(IOperationData operationData);
		string ExecuteTransaction(IOperationData operationData);
		}
	}