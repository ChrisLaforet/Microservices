using SagaBroker.Exception;
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

		internal SagaStateMachine(SagaOrchestrator orchestrator)
		{
			this.orchestrator = orchestrator;
			currentStage = orchestrator.RootStage;
		}

		private readonly SagaOrchestrator orchestrator;
		private ISagaStage currentStage;

		private string GenerateGUID()
		{
			return Guid.NewGuid().ToString();
		}

		public void Run(IOperationData operationData)
		{
			ISagaStage stage = currentStage;
			while (stage != null)
			{
				string transitionState;
				try
				{
					transitionState = ExecuteTransaction(operationData);
					rewindStack.Push(new RewindState(stage, operationData.Clone()));
				}
				catch (System.Exception)
				{
					Rewind();
					throw;
				}

				if (transitionState.Length == 0)
					stage = null;
				else
				{
					stage = orchestrator.GetStageByName(transitionState);
					if (stage == null)
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
							if ((orchestrator.RewindStrategyOptions & SagaOrchestrator.RewindStrategy.FailOnError) != 0)
								return;
							break;
					}
				}
				catch (System.Exception)
				{
					if ((orchestrator.RewindStrategyOptions & SagaOrchestrator.RewindStrategy.FailOnError) != 0)
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

			orchestrator.DBDriver.CreateSagaStep(sagaRecord);
			operationData.SagaRecordGUID = sagaRecord.GUID;

			string transitionName = string.Empty;

			try
			{
				using (TransactionScope transaction = new TransactionScope())
				{
					transitionName = currentStage.ExecuteTransaction(orchestrator.RemoteQueueDriver, operationData);
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

			orchestrator.DBDriver.CreateSagaStep(sagaRecord);
			operationData.SagaRecordGUID = sagaRecord.GUID;

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
