﻿using Apache.NMS;
using SagaProxy.Message;
using SagaProxy.QueueManagement;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SagaActiveMQ.Queue
{
	public class ActiveMQDriver : IQueueDriver
	{
		public const int DEFAULT_ACTIVEMQ_PORT = 61666;
		public const int DEFAULT_1_MINUTE_MESSAGE_TIMEOUTMS = 1 * 60 * 1000;

		private readonly IConnectionFactory activeMQfactory;
		private readonly int defaultMessageTimeoutMS;

		public ActiveMQDriver(string server, int port = DEFAULT_ACTIVEMQ_PORT, int defaultMessageTimeoutMS = DEFAULT_1_MINUTE_MESSAGE_TIMEOUTMS)
		{
			Uri connecturi = new Uri("activemq:tcp://" + server + ":" + port);
			activeMQfactory = new NMSConnectionFactory(connecturi);
			this.defaultMessageTimeoutMS = defaultMessageTimeoutMS;
		}

		public IQueueMessage ReceiveMessage(IQueueMessage sentMessage)
		{
			using (IConnection connection = activeMQfactory.CreateConnection())
			{
				using (ISession session = connection.CreateSession())
				{
					IDestination destination = session.GetQueue(sentMessage.ReplyQueueName);
					using (IMessageConsumer consumer = session.CreateConsumer(destination))
					{
						connection.Start();

						ITextMessage message = consumer.Receive() as ITextMessage;
						if (message == null)
							return null;

						return new ActiveMQMessage(sentMessage.RequestQueueName, sentMessage.ReplyQueueName, message.Text, message.NMSMessageId, message.NMSCorrelationID);
					}
				}
			}
		}

		public Task<IQueueMessage> ReceiveMessageAsync(string queueName)
		{
			throw new NotImplementedException();
		}

		public IQueueMessage SendMessage(IQueueMessage message)
		{
			using (IConnection connection = activeMQfactory.CreateConnection())
			{
				using (ISession session = connection.CreateSession())
				{
					IDestination destination = session.GetQueue(message.RequestQueueName);
					using (IMessageProducer producer = session.CreateProducer(destination))
					{
						connection.Start();
						producer.DeliveryMode = MsgDeliveryMode.Persistent;
						producer.RequestTimeout = new TimeSpan(0, 0, 0, defaultMessageTimeoutMS);

						ITextMessage request = session.CreateTextMessage(message.Content);
						request.NMSCorrelationID = message.CorrelationID;
						request.NMSMessageId = message.ID;

						producer.Send(request);

						return message;
					}
				}
			}
		}
	}
}
