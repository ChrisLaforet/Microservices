using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.Exception
{
	public class CyclicDependencyException : InvalidOperationException
	{
		public CyclicDependencyException(string message) : base(message)
		{ }
	}
}
