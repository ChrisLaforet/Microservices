using SagaBroker.Broker;
using SagaBroker.Exception;
using SagaBroker.StateMachine;
using System.Collections.Generic;

namespace SagaBroker.Saga
{
	public class SagaStage : ISagaStage
	{
		private readonly IList<ISagaStage> transitions = new List<ISagaStage>();

		public SagaStage(string name, BrokerStateNode brokerStateNode)
		{
			Name = name.ToUpper();
			BrokerStateNode = brokerStateNode;
		}

		private BrokerStateNode BrokerStateNode { set; get; }
		public string Name { private set; get; }

		public IStateNode StateNode
		{
			get
			{
				return BrokerStateNode;
			}
		}

		public void AddTransition(ISagaStage state)
		{
			transitions.Add(state);
		}

		public ISagaStage GetTransitionByName(string name)
		{
			foreach (ISagaStage stage in transitions)
			{
				if (stage.Name.CompareTo(name.ToUpper()) == 0)
					return stage;
			}
			throw new SagaTransitionException("Cannot locate a transition for " + name.ToUpper() + " from saga stage " + Name);
		}
	}
}
