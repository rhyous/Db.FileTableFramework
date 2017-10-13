using Rhyous.Db.FileTableFramework.Business;
using Rhyous.Db.FileTableFramework.Extensions;
using Rhyous.Db.FileTableFramework.Interfaces;
using Rhyous.Db.FileTableFramework.Managers;
using System;
using System.Data.SqlClient;
using System.IO;

namespace Rhyous.Db.FileTableFramework.Repos
{
    internal class FileTableRepo : IFileTableRepo
    {
        public Guid CreateFile(string table, string fileName, string pathId, byte[] data, SqlConnection conn)
        {
            SqlConnManager.IsConnected(conn);
            if (!FileTableExists(table, conn)) // This is used to prevent SQL injection
            {
                throw new Exception("Table does not exists or is not a FileTable.");
            }
            if (string.IsNullOrWhiteSpace(pathId))
            {
                pathId = HierarchyBuilder.NewChildHierarchyId(null);
            }
            var insertQry = "INSERT INTO {0} (name, file_stream, path_locator) "
                          + " OUTPUT Inserted.stream_id"
                          + " VALUES (@fileName, @data, HierarchyId::Parse(@pathId))";
            var qry = string.Format(insertQry, table);
            var cmd = new SqlCommand(qry, conn);
            cmd.Parameters.Add(new SqlParameter("@fileName", fileName));
            cmd.Parameters.Add(new SqlParameter("@data", data));
            cmd.Parameters.Add(new SqlParameter("@pathId", pathId));
            var streamId = (Guid)cmd.ExecuteScalar();
            return streamId;
        }

        public virtual string GetTableRootPath(string table, int option, SqlConnection conn)
        {
            SqlConnManager.IsConnected(conn);
            if (!FileTableExists(table, conn))
            {
                throw new Exception("Table does not exists or is not a FileTable.");
            }
            option = (option < 0 || option > 2) ? 0 : option;// Only allow valid options
            const string qry = "SELECT FileTableRootPath(@table, @option)";
            var tableRootCmd = new SqlCommand(qry, conn);
            tableRootCmd.Parameters.Add(new SqlParameter("@table", table));
            tableRootCmd.Parameters.Add(new SqlParameter("@option", option));
            var tableRoot = tableRootCmd.ExecuteScalar() as string;
            return tableRoot;
        }

        public virtual bool FileTableExists(string table, SqlConnection conn)
        {
            SqlConnManager.IsConnected(conn);
            const string qry = "SELECT Count(name) FROM Sys.Tables WHERE name = @table and is_filetable = 1";
            var cmd = new SqlCommand(qry, conn);
            cmd.Parameters.Add(new SqlParameter("@table", table));
            var result = (int)cmd.ExecuteScalar();
            return result == 1;
        }

        public virtual string CreateDirectory(string table, string dir, string pathId, SqlConnection conn)
        {
            SqlConnManager.IsConnected(conn);
            if (!FileTableExists(table, conn))
            {
                throw new Exception("Table does not exists or is not a FileTable.");
            }
            var CreateDirectoryQry = "INSERT INTO {0} (name, is_directory)"
                                            + " OUTPUT Inserted.path_locator.ToString()"
                                            + " VALUES (@dir, 1)";
            var qry = string.Format(CreateDirectoryQry, table);
            var cmd = new SqlCommand(qry, conn);
            cmd.Parameters.Add(new SqlParameter("@dir", dir));
            if (!string.IsNullOrWhiteSpace(pathId))
            {
                pathId = HierarchyBuilder.NewChildHierarchyId(pathId);
                CreateDirectoryQry = "INSERT INTO {0} (name, is_directory, path_locator)"
                    + " OUTPUT Inserted.path_locator.ToString()"
                    + " VALUES (@dir, 1, @pathId)";
                cmd.CommandText = string.Format(CreateDirectoryQry, table);
                cmd.Parameters.Add(new SqlParameter("@pathId", pathId as object));
            }
            pathId = cmd.ExecuteScalar() as string;
            return pathId;
        }

        public virtual string FindPath(string table, string path, bool isDirectory, SqlConnection conn)
        {
            SqlConnManager.IsConnected(conn);
            if (!FileTableExists(table, conn))
            {
                throw new Exception("Table does not exists or is not a FileTable.");
            }
            var tableRoot = GetTableRootPath(table, 0, conn);
            var tableRootFqdn = GetTableRootPath(table, 2, conn);
            if (IsTableRoot(path, tableRoot, tableRootFqdn))
            {
                return "/";
            }
            var relativePath = path.GetRelativePath(tableRoot);
            if (path.Length == relativePath.Length)
                relativePath = path.GetRelativePath(tableRootFqdn);
            var dirs = relativePath.SplitByDirectory();
            string pathLocator = null;
            foreach (var dir in dirs)
            {
                var qry1 = string.Format(GetPathLocatorQry, table, string.IsNullOrEmpty(pathLocator) ? "is null" : "= @pathLocator");
                SqlCommand cmd1 = new SqlCommand(qry1, conn);
                cmd1.Parameters.Add(new SqlParameter("@pathLocator", pathLocator));
                cmd1.Parameters.Add(new SqlParameter("@dir", dir));
                pathLocator = cmd1.ExecuteScalar() as string;
                if (string.IsNullOrWhiteSpace(pathLocator))
                    break;
            }
            return pathLocator;
        }

        internal bool IsTableRoot(string path, string tableRoot, string tableRootFqdn)
        {
            var tableRootRelative = Path.GetFileName(tableRoot);
            return string.IsNullOrEmpty(path)
                || path.Equals(tableRoot, StringComparison.InvariantCultureIgnoreCase)
                || path.Equals(tableRootFqdn, StringComparison.InvariantCultureIgnoreCase)
                || path.TrimStart('\\').Equals(tableRootRelative, StringComparison.InvariantCultureIgnoreCase);
        }

        private const string GetPathLocatorQry = "SELECT path_locator.ToString() FROM {0} WHERE parent_path_locator.ToString() {1} AND name = @dir";

        #region Dependency Injectable Properties

        public virtual ISqlConnectionManager SqlConnManager
        {
            get { return _SqlConnManager ?? (_SqlConnManager = new SqlConnectionManager()); }
            set { _SqlConnManager = value; }
        } private ISqlConnectionManager _SqlConnManager;

        public IHierarchyBuilder HierarchyBuilder
        {
            get { return _HierarchyBuilder ?? (_HierarchyBuilder = new HierarchyBuilder()); }
            set { _HierarchyBuilder = value; }
        } private IHierarchyBuilder _HierarchyBuilder;

        #endregion
    }
}
