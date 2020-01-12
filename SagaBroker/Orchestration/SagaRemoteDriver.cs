using SagaBroker.Exception;
using SagaBroker.Saga;
using SagaProxy.Message;
using SagaProxy.QueueManagement;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace SagaBroker.Orchestration
{
	internal class SagaRemoteDriver : ISagaRemoteDriver
		{
		private readonly IDictionary<string, RequestResponse> pending = new ConcurrentDictionary<string, RequestResponse>();

		private readonly Thread monitorDriver;

		private const int SLEEP_MSEC = 500 * 1000;

		internal SagaRemoteDriver(IQueueDriver driver)
		{
			QueueDriver = driver;
			monitorDriver = new Thread(this.ResponseMonitor);
			monitorDriver.IsBackground = true;
			monitorDriver.Start();
		}
		public IQueueDriver QueueDriver { get; private set; }

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
					IQueueMessage response = QueueDriver.ReceiveMessage(queue);
					if (response != null)
					{
						var node = pending[response.CorrelationID];
						if (node != null)
							node.Response = response;
						mustSleep = false;
					}
				}

				if (mustSleep)
				{
					ExpireOldMessages();
					Thread.Sleep(SLEEP_MSEC);
				}
			}
		}

		private void ExpireOldMessages()
		{
			DateTime now = DateTime.Now;
			foreach (var node in pending)
			{
				if (node.Value.ExpirationTimestamp.CompareTo(now) < 0)
					pending.Remove(node.Key);
			}
		}

		public string SendMessage(IQueueMessage message, int expirationMsec)
		{
			string correlationID = QueueDriver.SendMessage(message);
			pending.TryAdd(correlationID, new RequestResponse(message, expirationMsec));
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
		public RequestResponse(IQueueMessage request, int expirationMsec)
		{
			Request = request;
			StartTimestamp = DateTime.Now;
			ExpirationTimestamp = StartTimestamp.AddMilliseconds(expirationMsec);
		}

		public readonly DateTime StartTimestamp;
		public readonly DateTime ExpirationTimestamp;

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
