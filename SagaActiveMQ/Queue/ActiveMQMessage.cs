using SagaProxy.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaActiveMQ.QueueManagement
{
	public class ActiveMQMessage : IQueueMessage
	{
		public ActiveMQMessage(string ID, string correlationID, string content)
		{
			this.ID = ID;
			this.CorrelationID = correlationID;
			this.Content = content;
		}

		public string ID { get; set; }

		public string CorrelationID { get; set; }

		public string Content { get; set; }
	}
}
