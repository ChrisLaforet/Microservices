using SagaBroker.Exception;
using SagaBroker.StateMachine;
using System.Collections.Generic;

namespace SagaBroker.Saga
{
	public class SagaStage : ISagaStage
	{
		public SagaStage(string name, SagaOperation transaction, TransitionMap transactionTransitions, 
			SagaOperation compensatingTransaction = null)
		{
			Name = name.ToUpper();
			Transaction = transaction;
			TransactionTransitions = transactionTransitions;
			CompensatingTransaction = compensatingTransaction;
		}

		protected SagaOperation Transaction { private set; get; }
		protected TransitionMap TransactionTransitions { private set; get; }
		protected SagaOperation CompensatingTransaction { private set; get; }

		public string Name { private set; get; }

		public string TransactionTransitionOnSuccess
		{
			get
			{
				return TransactionTransitions.SuccessTransitionsTo;
			}
		}

		public string TransactionTransitionOnFailure
		{
			get
			{
				return TransactionTransitions.FailureTransitionsTo;
			}
		}

		public string TransactionTransitionOnExit
		{
			get
			{
				return TransactionTransitions.ExitTransitionsTo;
			}
		}

		public virtual string ExecuteTransaction(ISagaRemoteDriver sagaRemoteDriver, IOperationData operationData = null)
		{
			switch (Transaction(sagaRemoteDriver,operationData))
			{
				case StepState.STEP_SUCCESS:
					return TransactionTransitionOnSuccess;

				case StepState.STEP_FAILURE:
					return TransactionTransitionOnFailure;

				case StepState.STEP_EXIT:
					return TransactionTransitionOnExit;
			}
			return null;
		}

		public virtual StepState ExecuteCompensatingTransaction(ISagaRemoteDriver sagaRemoteDriver, IOperationData operationData = null)
		{
			if (CompensatingTransaction == null)
				return StepState.STEP_NODELEGATE;

			return CompensatingTransaction(sagaRemoteDriver,operationData);
		}
	}
}
