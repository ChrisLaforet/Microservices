using SagaBroker.Saga;
using SagaBroker.StateMachine;
using SagaProxy.QueueManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.Broker
{
	public abstract class BrokerStateNode : IStateNode
	{
		protected BrokerStateNode(SagaOperation transaction, SagaOperation compensatingTransaction = null)
		{
			Transaction = transaction;
			CompensatingTransaction = compensatingTransaction;
		}

		public SagaOperation Transaction { private set; get; }
		public SagaOperation CompensatingTransaction { private set; get; }

		public abstract BrokerData ExecuteTransaction(IOperationData operationData);
		public abstract BrokerData ExecuteCompensatingTransaction(CompensatingData compensatingData);
	}
}
