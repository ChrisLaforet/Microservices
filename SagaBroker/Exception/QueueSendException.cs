using System.IO;

namespace SagaBroker.Exception
{
	public class QueueSendException : IOException
	{
		public QueueSendException(string message) : base(message) { }

		public QueueSendException(string message, System.Exception innerException) : base(message, innerException) { }
	}
}
