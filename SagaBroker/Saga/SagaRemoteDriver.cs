using SagaProxy.QueueManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.Saga
{
	internal class SagaRemoteDriver
	{
		private readonly IQueueDriver queueDriver;

		internal SagaRemoteDriver(IQueueDriver driver)
		{
			queueDriver = driver;
		}
	}
}
