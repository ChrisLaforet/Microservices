using SagaBroker.Exception;
using SagaProxy.Message;
using SagaProxy.QueueManagement;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace SagaBroker.Saga
{
	internal class SagaRemoteDriver
	{
		private readonly IQueueDriver queueDriver;
		private readonly IDictionary<string, RequestResponse> pending = new ConcurrentDictionary<string, RequestResponse>();

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
			while (true)
			{
				bool mustSleep = true;

				IList<string> responseQueues = new List<string>();
				foreach (var node in pending)
				{
					if (node.Value.HasResponse)
						continue;

					if (!responseQueues.Contains(node.Value.Request.ReplyQueueName))
						responseQueues.Add(node.Value.Request.ReplyQueueName);
				}

				foreach (var queue in responseQueues)
				{
					IQueueMessage response = queueDriver.ReceiveMessage(queue);
					if (response != null)
					{
						var node = pending[response.CorrelationID];
						if (node != null)
							node.Response = response;
						mustSleep = false;
					}
				}

				if (mustSleep)
					Thread.Sleep(500 * 1000);
			}
		}

		public string SendMessage(IQueueMessage message)
		{
			string correlationID = queueDriver.SendMessage(message);
			pending.TryAdd(correlationID, new RequestResponse(message));
			return correlationID;
		}

		public IQueueMessage ReceiveResponse(string correlationID)
		{
			RequestResponse node = pending[correlationID];
			if (node == null)
				throw new QueueReadException("Unable to find a node for correlation ID " + correlationID);

			if (node.HasResponse)
			{
				IQueueMessage response = node.Response;
				pending.Remove(correlationID);
				return response;
			}

			return null;
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
