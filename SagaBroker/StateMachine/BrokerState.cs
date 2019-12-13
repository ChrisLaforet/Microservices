using SagaBroker.Saga;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.StateMachine
{
	public abstract class BrokerState
	{
		protected BrokerState(string name, ISagaOperation transaction, ISagaOperation compensatingTransaction)
		{
			Name = name.ToUpper();
			Transaction = transaction;
			CompensatingTransaction = compensatingTransaction;
		}

		public string Name { private set; get; }
		public ISagaOperation Transaction { private set; get; }
		public ISagaOperation CompensatingTransaction { private set; get; }
	}
}
