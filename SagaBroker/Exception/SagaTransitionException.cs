using System;

namespace SagaBroker.Exception
{
	public class SagaTransitionException : InvalidOperationException
	{
		public SagaTransitionException(string message) : base(message) { }

		public SagaTransitionException(string message, System.Exception innerException) : base(message, innerException) { }
	}
}
