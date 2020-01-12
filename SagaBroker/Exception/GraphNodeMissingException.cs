using System;

namespace SagaBroker.Exception
{
	public class GraphNodeMissingException : InvalidOperationException
	{
		public GraphNodeMissingException(string message) : base(message)
		{ }
	}
}
