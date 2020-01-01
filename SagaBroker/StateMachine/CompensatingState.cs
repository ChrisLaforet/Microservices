using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.StateMachine
{
	public class CompensatingState
	{
		public IStateNode StateNode { get; set; }
		public CompensatingData CompensatingData { get; set; }
	}
}
