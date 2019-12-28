using System;
using System.Collections.Generic;
using System.Text;

namespace SagaProxy.QueueManagement
{
	public interface IResponseData
	{
		string SagaRecordGUID { get; set; }
	}
}
