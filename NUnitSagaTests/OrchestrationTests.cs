using NUnit.Framework;
using SagaBroker.Exception;
using SagaBroker.Orchestration;
using SagaBroker.Saga;
using SagaBroker.StateMachine;
using SagaProxy.DBManagement;
using SagaProxy.Message;
using SagaProxy.QueueManagement;

namespace NUnitSagaTests
{
	public class OrchestrationTests
	{
		const string ORCHESTRATOR_NAME = "TestOrchestrator";

		[Test]
		public void WhenDefiningBasicOrchestrator_IfCheckName_ReturnsUppercaseName()
		{
			SagaOrchestrator orchestrator = new SagaOrchestrator(ORCHESTRATOR_NAME, null, null, null );
			Assert.AreEqual(ORCHESTRATOR_NAME.ToUpper(), orchestrator.Name);
		}

		[Test]
		public void WhenDefininingBasicOrchestrator_WithQueueDriver_ReturnsQueueDriverWrappedInSagaRemoteDriver()
		{
			IQueueDriver driver = new DummyQueueDriver();
			SagaOrchestrator orchestrator = new SagaOrchestrator(ORCHESTRATOR_NAME, driver, null, null);
			Assert.AreEqual(driver, orchestrator.RemoteQueueDriver.QueueDriver);
		}

		[Test]
		public void WhenDefininingBasicOrchestrator_WithDBDriver_ReturnsDBDriver()
		{
			IDBDriver driver = new DummyDBDriver();
			SagaOrchestrator orchestrator = new SagaOrchestrator(ORCHESTRATOR_NAME, null, driver, null);
			Assert.AreEqual(driver, orchestrator.DBDriver);
		}

		[Test]
		public void WhenDefininingBasicOrchestrator_WithoutStage_ThrowsExceptionOnOrchestrate()
		{
			SagaOrchestrator orchestrator = new SagaOrchestrator(ORCHESTRATOR_NAME, null, null, null);
			Assert.Throws<SagaTransitionException>(delegate { orchestrator.Orchestrate(null); });
		}

		[Test]
		public void WhenDefininingBasicOrchestrator_AddingOneStage_GetsStageAsRoot()
		{
			ISagaStage stage = new SagaStage(StageTests.STAGE_NAME, StageTests.FakeSuccessfulSagaOperation, StageTests.TestTransitionMap);
			SagaOrchestrator orchestrator = new SagaOrchestrator(ORCHESTRATOR_NAME, null, null, null);
			orchestrator.InsertStage(stage);
			Assert.AreEqual(stage.Name, orchestrator.RootStage.Name);
		}
	}

	class DummyQueueDriver : IQueueDriver
	{
		public IQueueMessage ReceiveMessage(IQueueMessage message)
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

	class DummyDBDriver : IDBDriver
	{
		public void CreateSagaStep(ISagaRecord record)
		{
			throw new System.NotImplementedException();
		}

		public ISagaRecord ReadSagaStep(string orchestratorName, string guid)
		{
			throw new System.NotImplementedException();
		}

		public void UpdateSagaStep(ISagaRecord record)
		{
			throw new System.NotImplementedException();
		}
	}
}
