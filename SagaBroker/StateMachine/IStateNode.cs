using SagaBroker.Broker;
using SagaBroker.Saga;
using SagaProxy.QueueManagement;

namespace SagaBroker.StateMachine
{
	public interface IStateNode
	{
		ISagaOperation Transaction { get; }
		ISagaOperation CompensatingTransaction { get; }

		BrokerData ExecuteTransaction(IOperationData operationData);
		BrokerData ExecuteCompensatingTransaction(CompensatingData compensatingData);

		BrokerData ProcessResponse(BrokerData brokerData, IResponseData responseData);
	}
}
