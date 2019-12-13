using System;
using System.Collections.Generic;
using System.Text;

namespace SagaBroker.DBManagement
{
	public interface IDBManager
	{
		void CreateSagaStep(string orchestratorName, string guid, string stageName);
		void UpdateSagaStep(string orchestratorName, string guid, string stateName);
		ISagaRecord ReadSagaStep(string orchestratorName, string guid);
	}
}
