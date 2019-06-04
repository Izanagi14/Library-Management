using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSysCore.DBConnection
{
	public class MySqlDBReader : IDisposable
	{
		private const string m_dbConnectionsString = @"Data Source=localhost;Database=libsys;User ID=root;password=";
		private MySqlConnection m_SqlConnection;
		public MySqlDataReader Reader { get; private set; }
		public MySqlDBReader(string query)
		{
			m_SqlConnection = new MySqlConnection(m_dbConnectionsString);
			m_SqlConnection.Open();
			Reader = new MySqlCommand(query, m_SqlConnection).ExecuteReader();
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					Reader.Close();
					m_SqlConnection.Close();
					Reader.Dispose();
					m_SqlConnection.Dispose();
				}
				disposedValue = true;
			}
		}
		public void Dispose()
		{
			Dispose(true);
		}
		#endregion


	}
}