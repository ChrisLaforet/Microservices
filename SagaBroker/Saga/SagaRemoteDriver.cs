using SagaProxy.Message;
using SagaProxy.QueueManagement;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SagaBroker.Saga
{
	internal class SagaRemoteDriver
	{
		private readonly IQueueDriver queueDriver;
		private readonly IDictionary<string, RequestResponse> pending = new Dictionary<string, RequestResponse>();

		private readonly Thread monitorDriver;

		internal SagaRemoteDriver(IQueueDriver driver)
		{
			queueDriver = driver;
			monitorDriver = new Thread(this.ResponseMonitor);
			monitorDriver.IsBackground = true;
			monitorDriver.Start();
		}

		private void ResponseMonitor()
		{

		}

		public void SendMessage(IQueueMessage message)
		{
			string correlationID = queueDriver.SendMessage(message);
			lock (pending)
			{
				pending.Add(correlationID, new RequestResponse(message));
			}

		}

	}

	class RequestResponse
	{
		public RequestResponse(IQueueMessage request)
		{
			Request = request;
		}

		public readonly DateTime Timestamp = DateTime.Now;
		public IQueueMessage Request { private set; get; }
		public IQueueMessage Response { set; get; }

		public string CorrelationID
		{
			get
			{
				return Request.CorrelationID;
			}
		}

		public bool HasResponse
		{
			get
			{
				return Response != null;
			}
		}
	}
}
