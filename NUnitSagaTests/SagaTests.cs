using NUnit.Framework;
using SagaBroker.Saga;
using SagaBroker.StateMachine;

namespace Tests
{
	public class SagaTests
	{
		[SetUp]
		public void Setup()
		{
		}


		private StepState FakeSuccessfulSagaOperation(ISagaRemoteDriver sagaRemoteDriver, IOperationData operationData)
		{
			return StepState.STEP_SUCCESS;
		}

		private StepState FakeFailureSagaOperation(ISagaRemoteDriver sagaRemoteDriver, IOperationData operationData)
		{
			return StepState.STEP_FAILURE;
		}

		[Test]
		public void WhenDefiningBasicSagaState_WithSuccessfulReturn_AssertSuccess()
		{
			ISagaStage stage = new SagaStage("Test", FakeSuccessfulSagaOperation);

			string nextStage = stage.ExecuteTransaction(null, null);
			Assert.AreSame(string.Empty, nextStage);
		}
	}
}