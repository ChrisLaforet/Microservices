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
		IResponseData ReceiveMessage(IQueueMessage message);
		Task<IResponseData> ReceiveMessageAsync(IQueueMessage message);
	}
}
