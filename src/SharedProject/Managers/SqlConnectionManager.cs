using Rhyous.Db.FileTableFramework.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Rhyous.Db.FileTableFramework.Managers
{
    internal class SqlConnectionManager : ISqlConnectionManager
    {
        public bool IsConnected(SqlConnection conn)
        {
            if (conn == null || conn.State != ConnectionState.Open)
            {
                throw new Exception("The Sql Connection must be open. It was " + conn?.State);
            }
            return true;
        }
    }
}
