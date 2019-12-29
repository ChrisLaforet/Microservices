using System;

namespace SagaBroker.Exception
{
	public class SagaException : InvalidOperationException
	{
		public SagaException(string message) : base(message) { }

		public SagaException(string message, System.Exception innerException) : base(message, innerException) { }
	}
}
