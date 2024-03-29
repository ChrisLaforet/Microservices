﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.StateMachine
{
	public class TransitionMap
	{
		public TransitionMap(string successTransitionsTo,string failureTransitionsTo,string exitTransitionsTo = null)
		{
			SuccessTransitionsTo = successTransitionsTo?.ToUpper();
			FailureTransitionsTo = failureTransitionsTo?.ToUpper();
			ExitTransitionsTo = exitTransitionsTo?.ToUpper();
		}

		public string SuccessTransitionsTo { private set; get; }
		public string FailureTransitionsTo { private set; get; }
		public string ExitTransitionsTo { private set; get; }
	}
}
