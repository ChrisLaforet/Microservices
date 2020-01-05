using SagaBroker.Saga;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.StateMachine
{
	public enum StepState
	{
		STEP_SUCCESS,
		STEP_FAILURE,
		STEP_EXIT,
		STEP_NODELEGATE
	}

	public delegate StepState SagaOperation(SagaRemoteDriver sagaRemoteDriver,IOperationData operationData);

	public interface ISagaStage
	{
		int ExpirationMsec { get; }
		string Name { get; }

		SagaOperation Operation { get; }
		SagaOperation RewindOperation { get; }
	}
}
