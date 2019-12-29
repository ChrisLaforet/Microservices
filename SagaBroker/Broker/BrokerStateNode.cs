﻿using SagaBroker.Saga;
using SagaBroker.StateMachine;
using SagaProxy.QueueManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.Broker
{
	public abstract class BrokerStateNode : IStateNode
	{
		protected BrokerStateNode(ISagaOperation transaction, ISagaOperation compensatingTransaction = null)
		{
			Transaction = transaction;
			CompensatingTransaction = compensatingTransaction;
		}

		public ISagaOperation Transaction { private set; get; }
		public ISagaOperation CompensatingTransaction { private set; get; }

		public abstract BrokerData ExecuteTransaction(IOperationData operationData);
		public abstract BrokerData ExecuteCompensatingTransaction(CompensatingData compensatingData);

		public BrokerData ProcessResponse(BrokerData brokerData, IResponseData responseData)
		{
			throw new NotImplementedException();
		}
	}
}
