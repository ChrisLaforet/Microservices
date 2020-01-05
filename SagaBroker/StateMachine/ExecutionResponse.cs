using SagaProxy.DBManagement;
using SagaProxy.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.StateMachine
{
	public abstract class ExecutionResponse
	{
		public StepState State { get; set; }
		public ISagaRecord SagaRecord { get; set; }
		public IQueueMessage SagaQueueMessage { get; set; }
		public object SagaData { get; set; }
		public string NextStep { get; set; }
	}
}
