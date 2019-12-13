using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker
{
	public abstract class StateBase
	{
		public BrokerState State { get;  }

		public string CommandQueueName { get; }

		public string ResponseQueueName { get; }

		public IList<TransitionState> Transitions { get;  }
	}
}
