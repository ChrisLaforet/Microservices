using System;
using SagaProxy.DBManagement;

namespace SagaBroker.Orchestration
	{
	internal class StateMachineSagaRecord : ISagaRecord
		{
		public string GUID { get; set; }
		public string OrchestratorName { get; set; }
		public string StageName { get; set; }
		public DateTime CreationTimestamp { get; set; }
		public DateTime? CompletionTimestamp { get; set; }
		public bool StageComplete { get; set; }
		public bool StageSuccess { get; set; }
		public bool StageRewinding { get; set; }
		}
	}
