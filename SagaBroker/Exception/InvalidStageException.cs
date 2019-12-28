using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.Exception
{
	public class InvalidStageException : InvalidOperationException
		{
			public InvalidStageException(string message) : base(message)
			{ }
		}
	}
