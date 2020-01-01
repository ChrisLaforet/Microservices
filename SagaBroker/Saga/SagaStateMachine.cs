using SagaBroker.Broker;
using SagaBroker.Exception;
using SagaBroker.StateMachine;
using SagaProxy.DBManagement;
using SagaProxy.Message;
using SagaProxy.QueueManagement;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace SagaBroker.Saga
{
	public class SagaStateMachine
	{
		private readonly Stack<CompensatingData> rewindStack = new Stack<CompensatingData>();

		internal SagaStateMachine(SagaOrchestrator orchestrator)
		{
			this.orchestrator = orchestrator;
			currentStage = orchestrator.FirstStage;
		}

		private readonly SagaOrchestrator orchestrator;
		private ISagaStage currentStage;

		private string GenerateGUID()
		{
			return Guid.NewGuid().ToString();
		}

		public BrokerData.StepState ExecuteTransaction(IOperationData operationData)
		{
			ISagaRecord sagaRecord = new StateMachineSagaRecord()
			{
				GUID = GenerateGUID(),
				OrchestratorName = orchestrator.Name,
				StageName = currentStage.Name,
				CreationTimestamp = DateTime.Now,
			};

			orchestrator.DBDriver.CreateSagaStep(sagaRecord);
			operationData.SagaRecordGUID = sagaRecord.GUID;

			try
			{
				using (TransactionScope transaction = new TransactionScope())
				{
					var transData = currentStage.StateNode.ExecuteTransaction(operationData);
					if (transData.State == BrokerData.StepState.STEP_EXIT)
						return transData.State;

					if (transData.State == BrokerData.StepState.STEP_FAILURE)
						throw new SagaException("Transaction failure in " + currentStage.Name);

					IResponseData response = null;

					if (transData.SagaQueueMessage != null)
					{
						transData.SagaQueueMessage.CorrelationID = sagaRecord.GUID;
						transData.SagaQueueMessage.ID = transData.SagaQueueMessage?.ID ?? GenerateGUID();
						orchestrator.QueueDriver.SendMessage(transData.SagaQueueMessage);

						response = orchestrator.QueueDriver.ReceiveMessage(transData.SagaQueueMessage);
					}

					var responseData = currentStage.StateNode.ProcessResponse(transData, response);

					if (transData.State == BrokerData.StepState.STEP_EXIT)
						return transData.State;

					if (transData.State == BrokerData.StepState.STEP_FAILURE)
						throw new SagaException("Response failure in " + currentStage.Name);

					currentStage = currentStage.GetTransitionByName(responseData.NextStep);

					transaction.Complete();
				}
			}
			catch (System.Exception)
			{
				sagaRecord.CompletionTimestamp = DateTime.Now;
				sagaRecord.StageComplete = true;
				sagaRecord.StageSuccess = false;
				orchestrator.DBDriver.UpdateSagaStep(sagaRecord);
// TODO: kick off rewind
				throw;
			}

			sagaRecord.CompletionTimestamp = DateTime.Now;
			sagaRecord.StageComplete = true;
			sagaRecord.StageSuccess = true;
			orchestrator.DBDriver.UpdateSagaStep(sagaRecord);

			return BrokerData.StepState.STEP_SUCCESS;
		}

		public BrokerData ExecuteCompensatingTransaction(CompensatingData compensatingData)
		{
return currentStage.StateNode.ExecuteCompensatingTransaction(compensatingData);
		}

	}

	class StateMachineSagaRecord : ISagaRecord
	{
		public string GUID { get; set; } 
		public string OrchestratorName { get; set; } 
		public string StageName { get; set; } 
		public DateTime CreationTimestamp { get; set; } 
		public DateTime? CompletionTimestamp { get; set; }
		public bool StageComplete { get; set; }
		public bool StageSuccess { get; set; }
	}
}
