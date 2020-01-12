using NUnit.Framework;
using SagaBroker.Saga;
using SagaBroker.StateMachine;

namespace NUnitSagaTests
{
	public class StageTests
	{
		public const string STAGE_NAME = "TestStage";
		public const string SUCCESS_TRANSITION_NAME = "Success";
		public const string FAILURE_TRANSITION_NAME = "Failure";
		public const string EXIT_TRANSITION_NAME = "Exit";


		[SetUp]
		public void Setup()
		{
		}


		public static StepState FakeSuccessfulSagaOperation(ISagaRemoteDriver sagaRemoteDriver, IOperationData operationData)
		{
			return StepState.STEP_SUCCESS;
		}

		public static StepState FakeFailureSagaOperation(ISagaRemoteDriver sagaRemoteDriver, IOperationData operationData)
		{
			return StepState.STEP_FAILURE;
		}

		public static  StepState FakeExitSagaOperation(ISagaRemoteDriver sagaRemoteDriver, IOperationData operationData)
		{
			return StepState.STEP_EXIT;
		}

		public static TransitionMap TestTransitionMap
		{
			get
			{
				return new TransitionMap(SUCCESS_TRANSITION_NAME, FAILURE_TRANSITION_NAME, EXIT_TRANSITION_NAME);
			}
		}

		[Test]
		public void WhenDefiningBasicSagaState_IfCheckName_ReturnsUppercasedName()
		{
			ISagaStage stage = new SagaStage(STAGE_NAME, FakeSuccessfulSagaOperation, TestTransitionMap);
			Assert.AreEqual(STAGE_NAME.ToUpper(), stage.Name);
		}

		[Test]
		public void WhenDefiningBasicSagaState_WithSuccessfulReturn_AssertUppercaseSuccessTransitionName()
		{
			ISagaStage stage = new SagaStage(STAGE_NAME, FakeSuccessfulSagaOperation, TestTransitionMap);

			string nextStage = stage.ExecuteTransaction(null, null);
			Assert.AreEqual(SUCCESS_TRANSITION_NAME.ToUpper(), nextStage);
		}

		[Test]
		public void WhenDefiningBasicSagaState_WithFaliureReturn_AssertUppercaseFailureTransitionName()
		{
			ISagaStage stage = new SagaStage(STAGE_NAME, FakeFailureSagaOperation, TestTransitionMap);

			string nextStage = stage.ExecuteTransaction(null, null);
			Assert.AreEqual(FAILURE_TRANSITION_NAME.ToUpper(), nextStage);
		}

		[Test]
		public void WhenDefiningBasicSagaState_WithExitReturn_AssertUppercaseExitTransitionName()
		{
			ISagaStage stage = new SagaStage(STAGE_NAME, FakeExitSagaOperation, TestTransitionMap);

			string nextStage = stage.ExecuteTransaction(null, null);
			Assert.AreEqual(EXIT_TRANSITION_NAME.ToUpper(), nextStage);
		}
	}
}