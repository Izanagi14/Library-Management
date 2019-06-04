using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibSysCore.DBConnection
{
	internal class MySqlDBConnection
	{
		private const string m_dbConnectionsString = @"Data Source=localhost;Database=libsys ;User ID=root;password=";
		public MySqlConnection SQLConnection { get; private set; }

		public MySqlDBConnection()
		{
			SQLConnection = new MySqlConnection(m_dbConnectionsString);
			SQLConnection = OpenConnection();
		}

		public MySqlCommand CreateCommand()
		{
			return SQLConnection.CreateCommand();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void RunInTrx(string query,ref MySqlConnection connection)
		{
			try
			{
				using (var trx = new MySqlTrx(connection))
				{
					var cmd = connection.CreateCommand();
					cmd.CommandText = query;
					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private MySqlConnection OpenConnection()
		{
			try
			{
				SQLConnection.Open();
				return SQLConnection;
			}
			catch (MySqlException mysqlException)
			{
				throw mysqlException;
			}
		}
	}
}