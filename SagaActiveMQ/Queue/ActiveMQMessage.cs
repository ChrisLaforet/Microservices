using SagaProxy.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaActiveMQ.Queue
{
	public class ActiveMQMessage : IQueueMessage
	{
		public ActiveMQMessage(string requestQueueName, string replyQueueName, string content, string ID = null, string correlationID = null)
		{
			this.ID = ID == null ? GenerateGUID() : ID;
			this.CorrelationID = correlationID == null ? GenerateGUID() : correlationID;
			this.Content = content;
			this.ReplyQueueName = replyQueueName;
			this.RequestQueueName = requestQueueName;
		}

		public ActiveMQMessage(IQueueMessage other)
		{
			this.ID = other.ID;
			this.CorrelationID = other.CorrelationID;
			this.Content = other.Content;
			this.ReplyQueueName = other.ReplyQueueName;
			this.RequestQueueName = other.RequestQueueName;
		}

		public string ID { get; set; }

		public string CorrelationID { get; set; }

		public string Content { get; set; }

		public string RequestQueueName { get; set; }
		public string ReplyQueueName { get; set; }

		private string GenerateGUID()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
