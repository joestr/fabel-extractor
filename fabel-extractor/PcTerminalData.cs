using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

namespace fabel_extractor
{
	class PcTerminalData
	{
		private OleDbConnection connection;
		private string connectionString, query;

		private List<FabelEntry> currentEntries = new List<FabelEntry>();

		public PcTerminalData(string connectionString, string query)
		{
			this.connectionString = connectionString;
			this.query = query;

			this.connection = new OleDbConnection(this.connectionString);
		}

		public List<FabelEntry> LoadAndSave()
		{
			this.currentEntries.RemoveAll(fE => fE.Date.Date.CompareTo(DateTime.Now.Date) < 1);

			connection.Open();

			OleDbCommand command = connection.CreateCommand();

			command.CommandText = this.query;

			OleDbDataReader reader = command.ExecuteReader();

			while (reader.Read())
			{
				currentEntries.Add(
					new FabelEntry(
						(int)reader.GetValue(0),
						(int)reader.GetValue(1),
						(DateTime)reader.GetValue(2),
						(string)reader.GetValue(3),
						(string)reader.GetValue(4),
						(DateTime)reader.GetValue(5),
						(DateTime)reader.GetValue(6),
						(int)reader.GetValue(7),
						reader.GetValue(8) is DBNull ? "" : (string)reader.GetValue(8),
						reader.GetValue(9) is DBNull ? "" : (string)reader.GetValue(9),
						reader.GetValue(10) is DBNull ? "" : (string)reader.GetValue(10),
						reader.GetValue(11) is DBNull ? "" : (string)reader.GetValue(11),
						reader.GetValue(12) is DBNull ? "" : (string)reader.GetValue(12),
						reader.GetValue(13) is DBNull ? "" : (string)reader.GetValue(13),
						reader.GetValue(14) is DBNull ? "" : (string)reader.GetValue(14),
						(int)reader.GetValue(15)
					)
				);
			}

			reader.Close();
			connection.Close();

			return this.currentEntries;
		}
	}
}
