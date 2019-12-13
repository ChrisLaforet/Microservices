using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SagaBroker.Exception
{
	public class QueueSendException : IOException
	{
		public QueueSendException(string message) : base(message) { }

		public QueueSendException(string message, Exception innerException) : base(message, innerException) { }
	}
}
