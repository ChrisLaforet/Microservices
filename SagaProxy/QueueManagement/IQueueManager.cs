using SagaProxy.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SagaProxy.QueueManagement
{
	public interface IQueueManager
	{
		string SendMessage(string queueName, IQueueMessage message);
		IQueueMessage ReceiveMessage(string queueName);
		Task<IQueueMessage> ReceiveMessageAsync(string queueName);
	}
}
