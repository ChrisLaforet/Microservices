using NSubstitute;
using NUnit.Framework;
using SagaBroker.Orchestration;
using SagaBroker.Saga;
using SagaBroker.StateMachine;
using SagaProxy.DBManagement;
using SagaProxy.Message;
using SagaProxy.QueueManagement;

namespace NUnitSagaTests
{
	public class StateMachineTests
	{
		const string NULL_STRING = (string)null;

		[Test]
		public void WhenDefiningAnOrchestratorWithOneStage_CallingRun_ReturnsSuccess()
		{
			var dbDriver = Substitute.For<IDBDriver>();
			var queueDriver = Substitute.For<IQueueDriver>();
			var sagaStage = Substitute.For<ISagaStage>();
			sagaStage.Name.Returns("A");
			sagaStage.TransactionTransitionOnSuccess.Returns(string.Empty);
			sagaStage.TransactionTransitionOnFailure.Returns(NULL_STRING);
			sagaStage.TransactionTransitionOnExit.Returns(NULL_STRING);

			var operationData = Substitute.For<IOperationData>();
			sagaStage.ExecuteTransaction(Substitute.For<ISagaRemoteDriver>(),operationData).ReturnsForAnyArgs(string.Empty);

			SagaOrchestrator orchestrator = new SagaOrchestrator(OrchestrationTests.ORCHESTRATOR_NAME, queueDriver, dbDriver, sagaStage);
			var steps = orchestrator.Orchestrate(operationData);
			Assert.AreEqual(1, steps);
		}

		[Test]
		public void WhenDefiningAnOrchestratorWithTwoStages_CallingRun_ReturnsSuccess()
		{
			var dbDriver = Substitute.For<IDBDriver>();
			var queueDriver = Substitute.For<IQueueDriver>();

			var sagaStageA = Substitute.For<ISagaStage>();
			sagaStageA.Name.Returns("A");
			sagaStageA.TransactionTransitionOnSuccess.Returns("B");
			sagaStageA.TransactionTransitionOnFailure.Returns(NULL_STRING);
			sagaStageA.TransactionTransitionOnExit.Returns(NULL_STRING);

			var sagaStageB = Substitute.For<ISagaStage>();
			sagaStageB.Name.Returns("B");
			sagaStageB.TransactionTransitionOnSuccess.Returns(string.Empty);
			sagaStageB.TransactionTransitionOnFailure.Returns(NULL_STRING);
			sagaStageB.TransactionTransitionOnExit.Returns(NULL_STRING);

			var operationData = Substitute.For<IOperationData>();
			sagaStageA.ExecuteTransaction(Substitute.For<ISagaRemoteDriver>(), operationData).ReturnsForAnyArgs("B");
			sagaStageB.ExecuteTransaction(Substitute.For<ISagaRemoteDriver>(), operationData).ReturnsForAnyArgs(string.Empty);

			SagaOrchestrator orchestrator = new SagaOrchestrator(OrchestrationTests.ORCHESTRATOR_NAME, queueDriver, dbDriver, sagaStageA);
			orchestrator.InsertStage(sagaStageB);
			var steps = orchestrator.Orchestrate(operationData);
			Assert.AreEqual(2, steps);
		}

		[Test]
		public void WhenDefiningAnOrchestratorWithTwoStagesFailingOnSecond_CallingRun_ReturnsFailure()
		{
			var dbDriver = Substitute.For<IDBDriver>();
			var queueDriver = Substitute.For<IQueueDriver>();

			var sagaStageA = Substitute.For<ISagaStage>();
			sagaStageA.Name.Returns("A");
			sagaStageA.TransactionTransitionOnSuccess.Returns("B");
			sagaStageA.TransactionTransitionOnFailure.Returns(NULL_STRING);
			sagaStageA.TransactionTransitionOnExit.Returns(NULL_STRING);

			var sagaStageB = Substitute.For<ISagaStage>();
			sagaStageB.Name.Returns("B");
			sagaStageB.TransactionTransitionOnSuccess.Returns(string.Empty);
			sagaStageB.TransactionTransitionOnFailure.Returns(NULL_STRING);
			sagaStageB.TransactionTransitionOnExit.Returns(NULL_STRING);

			var operationData = Substitute.For<IOperationData>();
			sagaStageA.ExecuteTransaction(Substitute.For<ISagaRemoteDriver>(), operationData).ReturnsForAnyArgs("B");
			//sagaStageB.When(x => x.ExecuteTransaction(Substitute.For<ISagaRemoteDriver>(), operationData))
			//	.Do(x => { throw new System.Exception(); });
			sagaStageB.ExecuteTransaction(Substitute.For<ISagaRemoteDriver>(), operationData)
				.ReturnsForAnyArgs(string.Empty)
				.AndDoes(x => { throw new System.Exception(); });
			SagaOrchestrator orchestrator = new SagaOrchestrator(OrchestrationTests.ORCHESTRATOR_NAME, queueDriver, dbDriver, sagaStageA);
			orchestrator.InsertStage(sagaStageB);
			Assert.Throws<System.Exception>(delegate { orchestrator.Orchestrate(operationData); });
		}

		[Test]
		public void WhenDefiningAnOrchestratorWithOneStage_CallingRun_WritesRecordsToDB()
		{
			var dbDriver = Substitute.For<IDBDriver>();
			var queueDriver = Substitute.For<IQueueDriver>();
			var sagaStage = Substitute.For<ISagaStage>();
			sagaStage.Name.Returns("A");
			sagaStage.TransactionTransitionOnSuccess.Returns(string.Empty);
			sagaStage.TransactionTransitionOnFailure.Returns(NULL_STRING);
			sagaStage.TransactionTransitionOnExit.Returns(NULL_STRING);

			var operationData = Substitute.For<IOperationData>();
			sagaStage.ExecuteTransaction(Substitute.For<ISagaRemoteDriver>(), operationData).ReturnsForAnyArgs(string.Empty);

			SagaOrchestrator orchestrator = new SagaOrchestrator(OrchestrationTests.ORCHESTRATOR_NAME, queueDriver, dbDriver, sagaStage);
			orchestrator.Orchestrate(operationData);
			dbDriver.ReceivedWithAnyArgs(1).CreateSagaStep(null);
			dbDriver.ReceivedWithAnyArgs(1).UpdateSagaStep(null);
		}

		[Test]
		public void WhenDefiningAnOrchestratorWithTwoStagesFailingOnSecond_CallingRun_WriteRecordsToDB()
		{
			var dbDriver = Substitute.For<IDBDriver>();
			var queueDriver = Substitute.For<IQueueDriver>();

			var sagaStageA = Substitute.For<ISagaStage>();
			sagaStageA.Name.Returns("A");
			sagaStageA.TransactionTransitionOnSuccess.Returns("B");
			sagaStageA.TransactionTransitionOnFailure.Returns(NULL_STRING);
			sagaStageA.TransactionTransitionOnExit.Returns(NULL_STRING);

			var sagaStageB = Substitute.For<ISagaStage>();
			sagaStageB.Name.Returns("B");
			sagaStageB.TransactionTransitionOnSuccess.Returns(string.Empty);
			sagaStageB.TransactionTransitionOnFailure.Returns(NULL_STRING);
			sagaStageB.TransactionTransitionOnExit.Returns(NULL_STRING);

			var operationData = Substitute.For<IOperationData>();
			sagaStageA.ExecuteTransaction(Substitute.For<ISagaRemoteDriver>(), operationData).ReturnsForAnyArgs("B");
			//sagaStageB.When(x => x.ExecuteTransaction(Substitute.For<ISagaRemoteDriver>(), operationData))
			//	.Do(x => { throw new System.Exception(); });
			sagaStageB.ExecuteTransaction(Substitute.For<ISagaRemoteDriver>(), operationData)
				.ReturnsForAnyArgs(string.Empty)
				.AndDoes(x => { throw new System.Exception(); });
			SagaOrchestrator orchestrator = new SagaOrchestrator(OrchestrationTests.ORCHESTRATOR_NAME, queueDriver, dbDriver, sagaStageA);
			orchestrator.InsertStage(sagaStageB);
			try
			{
				orchestrator.Orchestrate(operationData);
				Assert.Fail();
			}
			catch (System.Exception)
			{
				dbDriver.ReceivedWithAnyArgs(3).CreateSagaStep(null);
				dbDriver.ReceivedWithAnyArgs(3).UpdateSagaStep(null);
			}
		}
	}
}