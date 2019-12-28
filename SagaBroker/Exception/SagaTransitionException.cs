using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.Exception
{
	public class SagaTransitionException : InvalidOperationException
	{
		public SagaTransitionException(string message) : base(message) { }

		public SagaTransitionException(string message, System.Exception innerException) : base(message, innerException) { }
	}
}
