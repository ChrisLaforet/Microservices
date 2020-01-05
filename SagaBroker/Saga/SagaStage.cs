using SagaBroker.Exception;
using SagaBroker.StateMachine;
using System.Collections.Generic;

namespace SagaBroker.Saga
{
	public class SagaStage : ISagaStage
	{
		private readonly IList<ISagaStage> transitions = new List<ISagaStage>();

		public SagaStage(string name, SagaOperation transaction, TransitionMap transactionTransitions, SagaOperation compensatingTransaction = null, TransitionMap compensatingTransactionTransitions)
		{
			Name = name.ToUpper();
			Transaction = transaction;
			TransactionTransitions = transactionTransitions;
			CompensatingTransaction = compensatingTransaction;
			CompensationTransactionTransitions = compensatingTransactionTransitions;
		}

		protected SagaOperation Transaction { private set; get; }
		protected TransitionMap TransactionTransitions { private set; get; }
		protected SagaOperation CompensatingTransaction { private set; get; }
		protected TransitionMap CompensationTransactionTransitions { private set; get; }

		public string Name { private set; get; }

		public virtual string ExecuteTransaction(SagaRemoteDriver sagaRemoteDriver, IOperationData operationData = null)
		{
			switch (Transaction(sagaRemoteDriver,operationData))
			{
				case StepState.STEP_SUCCESS:
					return TransactionTransitions.SuccessTransitionsTo;

				case StepState.STEP_FAILURE:
					return TransactionTransitions.FailureTransitionsTo;

				case StepState.STEP_EXIT:
					return TransactionTransitions.ExitTransitionsTo;
			}
			return null;
		}

		public virtual string ExecuteCompensatingTransaction(SagaRemoteDriver sagaRemoteDriver, IOperationData operationData = null)
		{
			if (CompensatingTransaction == null)
				return string.Empty;

			switch (CompensatingTransaction(sagaRemoteDriver,operationData))
			{
				case StepState.STEP_SUCCESS:
					return CompensationTransactionTransitions.SuccessTransitionsTo;

				case StepState.STEP_FAILURE:
					return CompensationTransactionTransitions.FailureTransitionsTo;

				case StepState.STEP_EXIT:
					return CompensationTransactionTransitions.ExitTransitionsTo;
			}
			return null;
		}

		public void AddTransition(ISagaStage state)
		{
			transitions.Add(state);
		}

		public ISagaStage GetTransitionByName(string name)
		{
			foreach (ISagaStage stage in transitions)
			{
				if (stage.Name.CompareTo(name.ToUpper()) == 0)
					return stage;
			}
			throw new SagaTransitionException("Cannot locate a transition for " + name.ToUpper() + " from saga stage " + Name);
		}
	}
}
