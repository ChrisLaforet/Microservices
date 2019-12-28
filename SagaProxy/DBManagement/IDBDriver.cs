using System;
using System.Collections.Generic;
using System.Text;

namespace SagaProxy.DBManagement
{
	public interface IDBDriver
	{
		void CreateSagaStep(ISagaRecord record);
		void UpdateSagaStep(ISagaRecord record);
		ISagaRecord ReadSagaStep(string orchestratorName, string guid);
	}
}
