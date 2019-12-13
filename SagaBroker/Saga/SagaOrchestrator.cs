using SagaBroker.Exception;
using SagaBroker.StateMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.Saga
{
	public abstract class SagaOrchestrator
	{
		protected readonly IList<StateMachine.SagaState> states = new List<StateMachine.SagaState>();
		protected readonly HashSet<string> stateNames = new HashSet<string>();

		protected SagaOrchestrator(string orchestratorName)
		{
			Name = orchestratorName.ToUpper();
		}

		public string Name { protected set; get; }

		public void AddStageToSaga(StateMachine.SagaState stage)
		{
			// determine there are no cyclic dependencies
			foreach (var transition in stage.Transitions)
			{
				if (stateNames.Contains(transition.Name))
					throw new CyclicDependencyException(transition.Name + " in " + stage.BrokerState.Name);
			}

			states.Add(stage);
			stateNames.Add(stage.BrokerState.Name);
		}
	}
}
