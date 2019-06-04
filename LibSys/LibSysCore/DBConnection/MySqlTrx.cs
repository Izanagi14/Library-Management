using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Threading;

namespace LibSysCore.DBConnection
{
	public class MySqlTrx : IDisposable
	{
		public MySqlTransaction Trx { get; private set; }
		private static object m_mutex = new object();
		public MySqlTrx(MySqlConnection con)
		{
			Monitor.Enter(m_mutex);
			Trx = con.BeginTransaction(IsolationLevel.ReadUncommitted);
		}
		public void Dispose()
		{
			Trx.Commit();
			Monitor.Exit(m_mutex);
		}
	}
}