using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.QueueManagement
{
	public interface IQueueMessage
	{
		string CorrelationID { set; get; }
		string JsonMessage { set; get; }
	}
}
