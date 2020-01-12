using SagaProxy.Message;
using SagaProxy.QueueManagement;

namespace SagaBroker.Saga
	{
	public interface ISagaRemoteDriver
		{
		IQueueDriver QueueDriver { get; }

		IQueueMessage ReceiveResponse(string correlationID);

		string SendMessage(IQueueMessage message,int expirationMsec);
		}
	}