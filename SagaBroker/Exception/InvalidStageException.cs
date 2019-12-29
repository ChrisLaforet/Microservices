using System;

namespace SagaBroker.Exception
{
	public class InvalidStageException : InvalidOperationException
		{
			public InvalidStageException(string message) : base(message)
			{ }
		}
	}
