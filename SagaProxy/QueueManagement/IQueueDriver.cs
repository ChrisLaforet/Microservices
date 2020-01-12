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
		IQueueMessage ReceiveMessage(IQueueMessage messageWithReplyQueueName);
		IQueueMessage ReceiveMessage(string queueName);
	}
}
