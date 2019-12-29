using System.IO;

namespace SagaBroker.Exception
{
	public class QueueReadException : IOException
	{
		public QueueReadException(string message) : base(message) { }

		public QueueReadException(string message, System.Exception innerException) : base(message, innerException) { }
	}
}
