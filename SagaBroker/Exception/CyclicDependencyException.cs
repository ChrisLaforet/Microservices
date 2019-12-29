using System;

namespace SagaBroker.Exception
{
	public class CyclicDependencyException : InvalidOperationException
	{
		public CyclicDependencyException(string message) : base(message)
		{ }
	}
}
