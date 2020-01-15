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
		public const string ORCHESTRATOR_NAME = "TestOrchestrator";
		public const string STAGE2_NAME = "TestStage2";
		public const string STAGE3_NAME = "TestStage3";

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

		[Test]
		public void WhenDefininingBasicOrchestrator_AddingStageWithSameNameMultipleTimes_InsertThrowsException()
		{
			ISagaStage stage = new SagaStage(StageTests.STAGE_NAME, StageTests.FakeSuccessfulSagaOperation, StageTests.TestTransitionMap);
			SagaOrchestrator orchestrator = new SagaOrchestrator(ORCHESTRATOR_NAME, null, null, null);
			orchestrator.InsertStage(stage);
			Assert.Throws<CyclicDependencyException>(delegate { orchestrator.InsertStage(stage); });
		}

		[Test]
		public void WhenDefininingBasicOrchestrator_AddingStageThatReferencesItself_ValidationThrowsException()
		{
			TransitionMap transitionMap = new TransitionMap(StageTests.STAGE_NAME, null, null);
			ISagaStage stage = new SagaStage(StageTests.STAGE_NAME, StageTests.FakeSuccessfulSagaOperation, transitionMap);
			SagaOrchestrator orchestrator = new SagaOrchestrator(ORCHESTRATOR_NAME, null, null, null);
			orchestrator.InsertStage(stage);
			Assert.Throws<CyclicDependencyException>(delegate { orchestrator.ValidateStages(); });
		}

		[Test]
		public void WhenDefininingBasicOrchestrator_AddingMultipleStages_CanFindStages()
		{
			ISagaStage stage = new SagaStage(StageTests.STAGE_NAME, StageTests.FakeSuccessfulSagaOperation, StageTests.TestTransitionMap);
			ISagaStage stage2 = new SagaStage(STAGE2_NAME, StageTests.FakeSuccessfulSagaOperation, StageTests.TestTransitionMap);
			SagaOrchestrator orchestrator = new SagaOrchestrator(ORCHESTRATOR_NAME, null, null, null);
			orchestrator.InsertStage(stage);
			orchestrator.InsertStage(stage2);
			Assert.AreEqual(stage.Name, orchestrator.GetStageByName(stage.Name).Name);
			Assert.AreEqual(stage2.Name, orchestrator.GetStageByName(stage2.Name).Name);
		}

		[Test]
		public void WhenDefininingBasicOrchestrator_CheckingForNonexistentStageName_ReturnsNull()
		{
			ISagaStage stage = new SagaStage(StageTests.STAGE_NAME, StageTests.FakeSuccessfulSagaOperation, StageTests.TestTransitionMap);
			SagaOrchestrator orchestrator = new SagaOrchestrator(ORCHESTRATOR_NAME, null, null, null);
			orchestrator.InsertStage(stage);
			Assert.IsNull(orchestrator.GetStageByName("CANNOTFINDME"));
		}

		[Test]
		public void WhenDefininingBasicOrchestrator_AddingOneStage_FindsStageByUppercasingName()
		{
			ISagaStage stage = new SagaStage(StageTests.STAGE_NAME, StageTests.FakeSuccessfulSagaOperation, StageTests.TestTransitionMap);
			SagaOrchestrator orchestrator = new SagaOrchestrator(ORCHESTRATOR_NAME, null, null, null);
			orchestrator.InsertStage(stage);
			Assert.AreEqual(stage.Name, orchestrator.GetStageByName(StageTests.STAGE_NAME).Name);
		}

		[Test]
		public void WhenDefininingBasicOrchestrator_AddingMultipleStages_Contains2Transitions()
		{
			ISagaStage stage = new SagaStage(StageTests.STAGE_NAME, StageTests.FakeSuccessfulSagaOperation, StageTests.TestTransitionMap);
			ISagaStage stage2 = new SagaStage(STAGE2_NAME, StageTests.FakeSuccessfulSagaOperation, StageTests.TestTransitionMap);
			SagaOrchestrator orchestrator = new SagaOrchestrator(ORCHESTRATOR_NAME, null, null, null);
			orchestrator.InsertStage(stage);
			orchestrator.InsertStage(stage2);
			Assert.AreEqual(2, orchestrator.Transitions.Count);
		}

		[Test]
		public void WhenDefiningOrchestrator_AddingMultipleStagesWithTransitions_ValidateReturnsSuccess()
		{
			SagaOrchestrator orchestrator = new SagaOrchestrator(ORCHESTRATOR_NAME, null, null, null);
			orchestrator.InsertStage(new SagaStage("A", StageTests.FakeSuccessfulSagaOperation, new TransitionMap("B", "C", "D")));
			orchestrator.InsertStage(new SagaStage("B", StageTests.FakeSuccessfulSagaOperation, new TransitionMap("E", "F")));
			orchestrator.InsertStage(new SagaStage("C", StageTests.FakeSuccessfulSagaOperation, new TransitionMap("G", "H")));
			orchestrator.InsertStage(new SagaStage("D", StageTests.FakeSuccessfulSagaOperation, new TransitionMap(null, null)));
			orchestrator.InsertStage(new SagaStage("E", StageTests.FakeSuccessfulSagaOperation, new TransitionMap("I", "J")));
			orchestrator.InsertStage(new SagaStage("F", StageTests.FakeSuccessfulSagaOperation, new TransitionMap(null, null)));
			orchestrator.InsertStage(new SagaStage("G", StageTests.FakeSuccessfulSagaOperation, new TransitionMap(null, null)));
			orchestrator.InsertStage(new SagaStage("H", StageTests.FakeSuccessfulSagaOperation, new TransitionMap(null, null)));
			orchestrator.InsertStage(new SagaStage("I", StageTests.FakeSuccessfulSagaOperation, new TransitionMap("K", "L")));
			orchestrator.InsertStage(new SagaStage("J", StageTests.FakeSuccessfulSagaOperation, new TransitionMap(null, null)));
			orchestrator.InsertStage(new SagaStage("K", StageTests.FakeSuccessfulSagaOperation, new TransitionMap(null, null)));
			orchestrator.InsertStage(new SagaStage("L", StageTests.FakeSuccessfulSagaOperation, new TransitionMap(null, null)));

			orchestrator.ValidateStages();
			Assert.Pass();
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
