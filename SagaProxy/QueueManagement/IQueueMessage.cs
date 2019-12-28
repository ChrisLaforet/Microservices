using System;
using System.Collections.Generic;
using System.Text;

namespace SagaProxy.Message
{
	public interface IQueueMessage
	{
		string ID { get; set; }
		string CorrelationID { get; set; }
		string RequestQueueName { get; set; }
		string ReplyQueueName { get; set; }
		string Content { get; set; }
	}
}
