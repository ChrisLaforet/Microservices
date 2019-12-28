﻿using SagaProxy.DBManagement;
using SagaProxy.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.Broker
{
	public abstract class BrokerData
	{
		public enum StepState
		{
			STEP_SUCCESS,
			STEP_FAILURE,
			STEP_EXIT
		}

		public StepState State { get; set; }
		public ISagaRecord SagaRecord { get; set; }
		public IQueueMessage SagaQueueMessage { get; set; }
		public object SagaData { get; set; }
		public string NextStep { get; set; }
	}
}
