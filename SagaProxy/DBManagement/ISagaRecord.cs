using System;
using System.Collections.Generic;
using System.Text;

namespace SagaProxy.DBManagement
{
	public interface ISagaRecord
	{
		string GUID { get; set; }
		string OrchestratorName { get; set; }
		string StageName { get; set; }
		DateTime CreationTimestamp { get; set; }
		DateTime? CompletionTimestamp { get; set; }
		bool StageComplete { get; set; }
		bool StageSuccess { get; set; }
	}
}
