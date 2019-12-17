using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace SagaBroker.StateMachine
{
	public abstract class SagaState
	{
		private readonly IDictionary<String, TransitionState> transitionStates = new Dictionary<String, TransitionState>();

		public BrokerState BrokerState { get;  }
		public string ProxyName { get; }

		public IList<TransitionState> Transitions
		{
			get
			{
				return transitionStates.Values.ToImmutableList<TransitionState>();
			}
		}

		public virtual void AddTransitionState(TransitionState transitionState)
		{
			transitionStates.Remove(transitionState.Name);
			transitionStates.Add(transitionState.Name, transitionState);
		}

		public virtual void RemoveTransitionState(TransitionState transitionState)
		{
			transitionStates.Remove(transitionState.Name);
		}

	}
}
