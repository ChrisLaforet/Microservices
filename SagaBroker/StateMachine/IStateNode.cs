using SagaBroker.Broker;
using SagaBroker.Saga;
using SagaProxy.QueueManagement;

namespace SagaBroker.StateMachine
{
	public interface IStateNode
	{
		SagaOperation Transaction { get; }
		SagaOperation CompensatingTransaction { get; }

		BrokerData ExecuteTransaction(IOperationData operationData);
		BrokerData ExecuteCompensatingTransaction(CompensatingData compensatingData);

		BrokerData ProcessResponse(BrokerData brokerData, IResponseData responseData);
	}
}
