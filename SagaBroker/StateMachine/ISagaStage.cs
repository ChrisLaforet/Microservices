using SagaBroker.Saga;
using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.StateMachine
{
	public interface ISagaStage
	{
		string Name { get; }

		IStateNode StateNode { get; }

		ISagaStage GetTransitionByName(string name);
	}
}
