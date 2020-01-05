using SagaProxy.DBManagement;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security;
using System.Text;

namespace SagaSQLServer.DB
{
	public class SQLServerDriver : IDBDriver
	{
		private readonly string connectionString;
		private readonly SqlCredential credential;
		private readonly string tableName;

		public SQLServerDriver(string connectionString, string tableName, string userName = null, string password = null)
		{
			this.connectionString = connectionString;
			this.tableName = tableName;
			if (userName != null && password != null)
			{
				SecureString pwd = new SecureString();
				foreach (char ch in password)
					pwd.AppendChar(ch);
				this.credential = new SqlCredential(userName, pwd);
			}
		}

		public SQLServerDriver(string connectionString, string tableName, SqlCredential credential = null)
		{ 
			this.connectionString = connectionString;
			this.tableName = tableName;
			this.credential = credential;
		}

		private SqlConnection CreateConnection()
		{
			if (credential != null)
				return new SqlConnection(connectionString, credential);
			return new SqlConnection(connectionString);
		}

		public void CreateSagaStep(ISagaRecord record)
		{
			using (SqlConnection conn = CreateConnection())
			{
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = System.Data.CommandType.Text;
				cmd.CommandText = "INSERT INTO " + tableName + " (guid, orchestratorName, stageName, creationTimestamp, stageComplete, stageSuccess, stageRewinding) " +
					"VALUES (@GUID, @OrchestratorName, @StageName, @CreationTimestamp, 0, 0, @StageRewinding)";
				cmd.Parameters.Add("@GUID", System.Data.SqlDbType.VarChar);
				cmd.Parameters.Add("@OrchestratorName", System.Data.SqlDbType.VarChar);
				cmd.Parameters.Add("@StageName", System.Data.SqlDbType.VarChar);
				cmd.Parameters.Add("@CreationTimestamp", System.Data.SqlDbType.DateTime2);
				cmd.Parameters.Add("@StageRewinding", System.Data.SqlDbType.Bit);

				cmd.Parameters["@GUID"].Value = record.GUID;
				cmd.Parameters["@OrchestratorName"].Value = record.OrchestratorName;
				cmd.Parameters["@StageName"].Value = record.StageName;
				cmd.Parameters["@CreationTimestamp"].Value = record.CreationTimestamp;
				cmd.Parameters["@StageRewinding"].Value = record.StageRewinding ? 1 : 0;

				conn.Open();
				cmd.ExecuteNonQuery();
			}
		}

		public ISagaRecord ReadSagaStep(string orchestratorName, string guid)
		{
			using (SqlConnection conn = CreateConnection())
			{
				SqlCommand cmd = new SqlCommand("SELECT " +
					"guid, orchestratorName, stageName, creationTimestamp, completionTimestamp, stageComplete, stageSuccess, stageRewinding FROM " + 
					tableName + " WHERE guid = @GUID AND orchestratorName = @OrchestratorName", conn);
				cmd.Parameters.Add("@GUID", System.Data.SqlDbType.VarChar);
				cmd.Parameters.Add("@OrchestratorName", System.Data.SqlDbType.VarChar);
				cmd.Parameters["@GUID"].Value = guid;
				cmd.Parameters["@OrchestratorName"].Value = orchestratorName;

				conn.Open();

				SqlDataReader reader = cmd.ExecuteReader();
				try
				{

					if (reader.Read())
					{
						return new SagaRecord()
						{
							GUID = guid,
							OrchestratorName = orchestratorName,
							StageName = reader.GetString(2),
							CreationTimestamp = reader.GetDateTime(3),
							CompletionTimestamp = (reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4)),
							StageComplete = reader.GetBoolean(5),
							StageSuccess = reader.GetBoolean(6),
							StageRewinding = reader.GetBoolean(7)
						};
					}
				}
				finally
				{
					reader.Close();
				}
			}

			return null;
		}

		public void UpdateSagaStep(ISagaRecord record)
		{
			using (SqlConnection conn = CreateConnection())
			{
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = System.Data.CommandType.Text;
				cmd.CommandText = "UPDATE " + tableName + " SET stageName = @StageName, creationTimestamp = @CreationTimestamp," +
					"completionTimestamp = @CompletionTimestamp,stageComplete = @StageComplete, stageSuccess = @StageSuccess, " +
					"stageRewinding = @StageRewinding " +
					"WHERE guid = @GUID AND orchestratorName = @OrchestratorName";
				cmd.Parameters.Add("@StageName", System.Data.SqlDbType.VarChar);
				cmd.Parameters.Add("@CreationTimestamp", System.Data.SqlDbType.DateTime2);
				cmd.Parameters.Add("@CompletionTimestamp", System.Data.SqlDbType.DateTime2);
				cmd.Parameters.Add("@StageComplete", System.Data.SqlDbType.Bit);
				cmd.Parameters.Add("@StageSuccess", System.Data.SqlDbType.Bit);
				cmd.Parameters.Add("@StageRewinding", System.Data.SqlDbType.Bit);
				cmd.Parameters.Add("@GUID", System.Data.SqlDbType.VarChar);
				cmd.Parameters.Add("@OrchestratorName", System.Data.SqlDbType.VarChar);

				cmd.Parameters["@StageName"].Value = record.StageName;
				cmd.Parameters["@CreationTimestamp"].Value = record.CreationTimestamp;
				if (record.CompletionTimestamp == null)
					cmd.Parameters["@CompletionTimestamp"].Value = DBNull.Value;
				else
					cmd.Parameters["@CompletionTimestamp"].Value = record.CompletionTimestamp;
				cmd.Parameters["@StageComplete"].Value = record.StageComplete ? 1 : 0;
				cmd.Parameters["@StageSuccess"].Value = record.StageSuccess ? 1 : 0;
				cmd.Parameters["@StageRewinding"].Value = record.StageRewinding ? 1 : 0;
				cmd.Parameters["@GUID"].Value = record.GUID;
				cmd.Parameters["@OrchestratorName"].Value = record.OrchestratorName;

				conn.Open();
				cmd.ExecuteNonQuery();
			}
		}
	}

	class SagaRecord : ISagaRecord
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
