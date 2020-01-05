using SagaProxy.Message;

namespace SagaBroker.Saga
	{
	public interface ISagaRemoteDriver
		{
		IQueueMessage ReceiveResponse(string correlationID);
		string SendMessage(IQueueMessage message,int expirationMsec);
		}
	}