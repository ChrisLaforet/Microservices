using System;
using System.Collections.Generic;
using System.Text;

namespace SagaProxy.QueueManagement
{
	public interface IQueueMessage
	{
		string CorrelationID { set; get; }
		string JsonMessage { set; get; }
	}
}
