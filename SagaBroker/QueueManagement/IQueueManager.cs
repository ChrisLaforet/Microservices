using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.QueueManagement
{
	public interface IQueueManager
	{
		String SendMessage(String QueueName, IQueueMessage Message);
		IQueueMessage ReceiveMessage(String QueueName);
	}
}
