using SagaBroker.Exception;
using SagaBroker.Saga;
using SagaBroker.StateMachine;
using SagaProxy.DBManagement;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace SagaBroker.Orchestration
{
	internal class SagaStateMachine : ISagaStateMachine
	{
		private readonly Stack<RewindState> rewindStack = new Stack<RewindState>();

		public SagaStateMachine(ISagaOrchestrator orchestrator)
		{
			this.orchestrator = orchestrator;
			currentStage = orchestrator.RootStage;
		}

		private readonly ISagaOrchestrator orchestrator;
		private ISagaStage currentStage;

		public int StagesExecuted { get; private set; } = 0;

		private string GenerateGUID()
		{
			return Guid.NewGuid().ToString();
		}

		public void Run(IOperationData operationData)
		{
			while (currentStage != null)
			{
				string transitionState;
				try
				{
					transitionState = ExecuteTransaction(operationData);
					rewindStack.Push(new RewindState(currentStage, operationData.Clone()));
				}
				catch (System.Exception)
				{
					Rewind();
					throw;
				}

				if (transitionState == null || transitionState.Length == 0)
					currentStage = null;
				else
				{
					currentStage = orchestrator.GetStageByName(transitionState);
					if (currentStage == null)
						throw new SagaTransitionException("Unable to locate transition stage named " + transitionState);
				}
			}
		}

		private void Rewind()
		{
			while (rewindStack.Count > 0)
			{
				RewindState rewindState = rewindStack.Pop();
				try
				{
					currentStage = rewindState.Stage;
					switch (ExecuteCompensatingTransaction(rewindState.OperationData))
					{
						case StepState.STEP_EXIT:
							return;

						case StepState.STEP_FAILURE:
							if ((orchestrator.RewindStrategyOptions & RewindStrategy.FailOnError) != 0)
								return;
							break;
					}
				}
				catch (System.Exception)
				{
					if ((orchestrator.RewindStrategyOptions & RewindStrategy.FailOnError) != 0)
						return;
				}
			}
		}

		public string ExecuteTransaction(IOperationData operationData)
		{
			ISagaRecord sagaRecord = new StateMachineSagaRecord()
			{
				GUID = GenerateGUID(),
				OrchestratorName = orchestrator.Name,
				StageName = currentStage.Name,
				CreationTimestamp = DateTime.Now,
				StageComplete = false,
				StageRewinding = false,
				StageSuccess = false
			};

			operationData.SagaRecordGUID = sagaRecord.GUID;
			orchestrator.DBDriver.CreateSagaStep(sagaRecord);

			string transitionName = string.Empty;

			try
			{
				using (TransactionScope transaction = new TransactionScope())
				{
					transitionName = currentStage.ExecuteTransaction(orchestrator.RemoteQueueDriver, operationData);
					++StagesExecuted;
					if (transitionName == null)
					{
						throw new SagaException("Error encountered while processing step " + currentStage.Name);
					}

					//var transData = currentStage.StateNode.ExecuteTransaction(operationData);
					//if (transData.State == ExecutionResponse.StepState.STEP_EXIT)
					//	return transData.State;

					//if (transData.State == ExecutionResponse.StepState.STEP_FAILURE)
					//	throw new SagaException("Transaction failure in " + currentStage.Name);

					//IResponseData response = null;

					//if (transData.SagaQueueMessage != null)
					//{
					//	transData.SagaQueueMessage.CorrelationID = sagaRecord.GUID;
					//	transData.SagaQueueMessage.ID = transData.SagaQueueMessage?.ID ?? GenerateGUID();
					//	orchestrator.RemoteQueueDriver.SendMessage(transData.SagaQueueMessage, operationData.ExpirationMsec);

					//	response = orchestrator.RemoteQueueDriver.ReceiveMessage(transData.SagaQueueMessage);
					//}

					//var responseData = currentStage.StateNode.ProcessResponse(transData, response);
					transaction.Complete();
				}
			}
			catch (System.Exception)
			{
				sagaRecord.CompletionTimestamp = DateTime.Now;
				sagaRecord.StageComplete = true;
				sagaRecord.StageSuccess = false;
				orchestrator.DBDriver.UpdateSagaStep(sagaRecord);
				throw;
			}

			sagaRecord.CompletionTimestamp = DateTime.Now;
			sagaRecord.StageComplete = true;
			sagaRecord.StageSuccess = true;
			orchestrator.DBDriver.UpdateSagaStep(sagaRecord);

			return transitionName;
		}

		public StepState ExecuteCompensatingTransaction(IOperationData operationData)
		{
			ISagaRecord sagaRecord = new StateMachineSagaRecord()
			{
				GUID = GenerateGUID(),
				OrchestratorName = orchestrator.Name,
				StageName = currentStage.Name,
				CreationTimestamp = DateTime.Now,
				StageComplete = false,
				StageRewinding = false,
				StageSuccess = false
			};

			operationData.SagaRecordGUID = sagaRecord.GUID;
			orchestrator.DBDriver.CreateSagaStep(sagaRecord);

			StepState compensatingState;

			try
			{
				using (TransactionScope transaction = new TransactionScope())
				{
					compensatingState = currentStage.ExecuteCompensatingTransaction(orchestrator.RemoteQueueDriver, operationData);
					transaction.Complete();
				}
			}
			catch (System.Exception)
			{
				sagaRecord.CompletionTimestamp = DateTime.Now;
				sagaRecord.StageComplete = true;
				sagaRecord.StageSuccess = false;
				orchestrator.DBDriver.UpdateSagaStep(sagaRecord);
				throw;
			}

			sagaRecord.CompletionTimestamp = DateTime.Now;
			sagaRecord.StageComplete = true;
			sagaRecord.StageSuccess = true;
			orchestrator.DBDriver.UpdateSagaStep(sagaRecord);

			return compensatingState;
		}
	}

	class RewindState
	{
		public RewindState(ISagaStage stage, IOperationData operationData)
		{
			this.Stage = stage;
			this.OperationData = operationData;
		}

		public ISagaStage Stage { get; private set; }
		public IOperationData OperationData { get; private set; }
	}
}
