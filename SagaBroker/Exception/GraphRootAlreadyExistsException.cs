using System;

namespace SagaBroker.Exception
{
	public class GraphRootAlreadyExistsException : InvalidOperationException
	{
		public GraphRootAlreadyExistsException(string message) : base(message)
		{ }
	}
}
