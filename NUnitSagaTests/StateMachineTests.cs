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
			orchestrator.Orchestrate(operationData);
			Assert.Pass();
		}
	}

	class TestQueueDriver : IQueueDriver
	{
		public IQueueMessage ReceiveMessage(IQueueMessage messageWithReplyQueueName)
		{
			throw new System.NotImplementedException();
		}

		public IQueueMessage ReceiveMessage(string queueName)
		{
			throw new System.NotImplementedException();
		}

		public string SendMessage(IQueueMessage message)
		{
			throw new System.NotImplementedException();
		}
	}
}