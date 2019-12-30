using SagaProxy.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SagaProxy.QueueManagement
{
	public interface IQueueDriver
	{
		string SendMessage(IQueueMessage message);
		IQueueMessage ReceiveMessage(IQueueMessage message);
		IQueueMessage ReceiveMessage(string queueName);
//		Task<IResponseData> ReceiveMessageAsync(IQueueMessage message);
	}
}
