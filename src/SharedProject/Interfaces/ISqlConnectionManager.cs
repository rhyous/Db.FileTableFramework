using System.Data.SqlClient;

namespace Rhyous.Db.FileTableFramework.Interfaces
{
    public interface ISqlConnectionManager
    {
        bool IsConnected(SqlConnection conn);
    }
}
