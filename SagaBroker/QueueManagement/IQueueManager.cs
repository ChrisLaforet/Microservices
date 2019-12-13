using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.QueueManagement
{
	public interface IQueueManager
	{
		public String SendMessage(String QueueName, IQueueMessage Message);
		public IQueueMessage ReceiveMessage(String QueueName);
	}
}
