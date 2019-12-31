
using SagaBroker.StateMachine;

namespace SagaBroker.Saga
{
	public interface ISagaOperation : IOperationData
	{
		int ExpirationMsec { get; }
		void OperationCallback();
	}
}
