using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.StateMachine
{
	public interface IOperationData
	{
		string SagaRecordGUID { get; set; }
	}
}
