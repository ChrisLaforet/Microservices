using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.QueueManagement
{
	public interface IQueueManager
	{
		string SendMessage(string QueueName, IQueueMessage Message);
		IQueueMessage ReceiveMessage(string QueueName);
	}
}
