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

		public PcTerminalData(string connectionString, string query)
		{
			this.connectionString = connectionString;
			this.query = query;

			this.connection = new OleDbConnection(this.connectionString);
		}

		public List<IList<object>> GetDataAsTableForSpreadsheet()
		{
			connection.Open();

			OleDbCommand command = connection.CreateCommand();

			command.CommandText = this.query;

			OleDbDataReader reader = command.ExecuteReader();

			StreamWriter buff = new StreamWriter(new MemoryStream());

			var table = new List<IList<object>>();

			var row = new List<object>();

			for (int fieldNumber = 0; fieldNumber < reader.FieldCount; fieldNumber++)
			{
				row.Add(reader.GetName(fieldNumber));
			}

			table.Add(row);

			while (reader.Read())
			{
				row = new List<object>();
				for (int fieldNumber = 0; fieldNumber < reader.FieldCount; fieldNumber++)
				{
					row.Add(reader.GetValue(fieldNumber));
				}
				table.Add(row);
			}

			reader.Close();
			connection.Close();

			return table;
		}
	}
}
