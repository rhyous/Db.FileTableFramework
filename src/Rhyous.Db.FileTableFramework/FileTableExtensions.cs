using Microsoft.SqlServer.Server;
using Rhyous.Db.FileTableFramework.Interfaces;
using Rhyous.Db.FileTableFramework.Managers;
using Rhyous.Db.FileTableFramework.Repos;
using System;
using System.Data.SqlClient;

namespace Rhyous.Db.FileTableFramework
{
    public class FileTableExtensions
    {
        [SqlFunction(DataAccess = DataAccessKind.Read, SystemDataAccess = SystemDataAccessKind.Read)]
        public static bool FileTableExists(string table)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                return _FileTableExists(table, conn);
            }
        }

        internal static bool _FileTableExists(string table, SqlConnection conn, IFileTableRepo repo = null)
        {
            return (repo ?? new FileTableRepo()).FileTableExists(table, conn);
        }

        [SqlFunction(DataAccess = DataAccessKind.Read, SystemDataAccess = SystemDataAccessKind.Read)]
        public static string DirectoryExists(string table, string path)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                return _DirectoryExists(table, path, conn);
            }
        }

        internal static string _DirectoryExists(string table, string path, SqlConnection conn, IFileTableManager ftManager = null)
        {
            return (ftManager ?? new DirectoryManager()).DirectoryExists(table, path, conn);
        }

        [SqlFunction(DataAccess = DataAccessKind.Read, SystemDataAccess = SystemDataAccessKind.Read)]
        public static string FileExists(string table, string path)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                return _FileExists(table, path, conn);
            }
        }

        internal static string _FileExists(string table, string path, SqlConnection conn, IFileTableManager ftManager = null)
        {
            return (ftManager ?? new DirectoryManager()).FileExists(table, path, conn);
        }

        [SqlProcedure()]
        public static void CreateDirectory(string table, string path, out string id)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                id = _CreateDirectory(table, path, conn);
            }
        }

        internal static string _CreateDirectory(string table, string path, SqlConnection conn, IFileTableManager ftManager = null)
        {
            return (ftManager ?? new DirectoryManager()).CreateDirectory(table, path, conn);
        }

        [SqlProcedure()]
        public static void CreateFile(string table, string path, byte[] data, out Guid id)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                id = _CreateFile(table, path, data, conn);
            }
        }

        internal static Guid _CreateFile(string table, string path, byte[] data, SqlConnection conn, IFileTableManager ftManager = null)
        {
            return (ftManager ?? new DirectoryManager()).CreateFile(table, path, data, conn);
        }
    }
}
