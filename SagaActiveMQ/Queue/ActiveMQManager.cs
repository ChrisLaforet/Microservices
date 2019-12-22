using Apache.NMS;
using SagaProxy.Message;
using SagaProxy.QueueManagement;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SagaActiveMQ.QueueManagement
{
	public class ActiveMQManager : IQueueManager
	{
		public const int DEFAULT_ACTIVEMQ_PORT = 61666;
		public const int DEFAULT_24HR_MESSAGE_TIMEOUTMS = 24 * 60 * 60 * 1000;

		private readonly IConnectionFactory activeMQfactory;
		private readonly int defaultMessageTimeoutMS;

		public ActiveMQManager(string server, int port = DEFAULT_ACTIVEMQ_PORT, int defaultMessageTimeoutMS = DEFAULT_24HR_MESSAGE_TIMEOUTMS)
		{
			Uri connecturi = new Uri("activemq:tcp://" + server + ":" + port);
			activeMQfactory = new NMSConnectionFactory(connecturi);
			this.defaultMessageTimeoutMS = defaultMessageTimeoutMS;
		}

		public IQueueMessage ReceiveMessage(string queueName)
		{
			using (IConnection connection = activeMQfactory.CreateConnection())
			{
				using (ISession session = connection.CreateSession())
				{
					IDestination destination = session.GetQueue(queueName);
					using (IMessageConsumer consumer = session.CreateConsumer(destination))
					{
						connection.Start();

						ITextMessage message = consumer.Receive() as ITextMessage;
						if (message == null)
							return null;

						return new ActiveMQMessage(message.NMSMessageId, message.NMSCorrelationID, message.Text);
					}
				}
			}
		}

		public Task<IQueueMessage> ReceiveMessageAsync(string queueName)
		{
			throw new NotImplementedException();
		}

		public string SendMessage(string queueName, IQueueMessage message)
		{
			using (IConnection connection = activeMQfactory.CreateConnection())
			{
				using (ISession session = connection.CreateSession())
				{
					IDestination destination = session.GetQueue(queueName);
					using (IMessageProducer producer = session.CreateProducer(destination))
					{
						connection.Start();
						producer.DeliveryMode = MsgDeliveryMode.Persistent;
						producer.RequestTimeout = new TimeSpan(0, 0, 0, defaultMessageTimeoutMS);

						string correlationID = GenerateCorrelationID();
						ITextMessage request = session.CreateTextMessage(message.Content);
						request.NMSCorrelationID = correlationID;

						producer.Send(request);

						return correlationID;
					}
				}
			}
		}

		private string GenerateCorrelationID()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
