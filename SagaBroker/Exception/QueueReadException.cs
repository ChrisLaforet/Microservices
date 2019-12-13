using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SagaBroker.Exception
{
	public class QueueReadException : IOException
	{
		public QueueReadException(string message) : base(message) { }

		public QueueReadException(string message, Exception innerException) : base(message, innerException) { }
	}
}
