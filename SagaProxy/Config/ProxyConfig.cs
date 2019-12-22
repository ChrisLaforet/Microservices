using System;
using System.Collections.Generic;
using System.Text;

namespace SagaProxy.Config
{
	public class ProxyConfig
	{
		public string Name { get; }
		public string CommandQueueName { get; }
		public string ResponseQueueName { get; }
	}
}
