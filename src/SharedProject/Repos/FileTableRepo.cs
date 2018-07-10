using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using Rhyous.Db.FileTableFramework.Business;
using Rhyous.Db.FileTableFramework.Extensions;
using Rhyous.Db.FileTableFramework.Interfaces;
using Rhyous.Db.FileTableFramework.Managers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Rhyous.Db.FileTableFramework.Repos
{
    internal class FileTableRepo : IFileTableRepo
    {
        public Guid CreateFile(string table, string fileName, SqlHierarchyId pathId, byte[] data, SqlConnection conn)
        {
            SqlConnManager.IsConnected(conn);
            if (!FileTableExists(table, conn)) // This is used to prevent SQL injection
            {
                throw new Exception("Table does not exists or is not a FileTable.");
            }
            if (pathId.IsNull)
            {
                pathId = HierarchyBuilder.NewChildHierarchyId(SqlHierarchyId.Null);
            }
            var insertQry = "INSERT INTO {0} (name, file_stream, path_locator) "
                          + " OUTPUT Inserted.stream_id"
                          + " VALUES (@fileName, @data, @pathId)";
            var qry = string.Format(insertQry, table);
            var cmd = new SqlCommand(qry, conn);
            cmd.Parameters.Add(new SqlParameter("@fileName", fileName));
            cmd.Parameters.Add(new SqlParameter("@data", data));
            cmd.Parameters.Add(new SqlParameter("@pathId", pathId) { UdtTypeName = Constants.HierarchyId });
            var streamId = (Guid)cmd.ExecuteScalar();
            PipeScalar(SqlDbType.UniqueIdentifier, streamId);
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
            cmd.Parameters.Add(new SqlParameter("@table", table.Trim('[', ']')));
            var result = (int)cmd.ExecuteScalar();
            return result == 1;
        }

        public virtual SqlHierarchyId CreateDirectory(string table, string dir, SqlHierarchyId pathId, SqlConnection conn, bool pipeToOutput = false)
        {
            SqlConnManager.IsConnected(conn);
            if (!FileTableExists(table, conn))
            {
                throw new Exception("Table does not exists or is not a FileTable.");
            }
            var CreateDirectoryQry = "INSERT INTO {0} (name, is_directory)"
                                            + " OUTPUT Inserted.path_locator"
                                            + " VALUES (@dir, 1)";
            var qry = string.Format(CreateDirectoryQry, table);
            var cmd = new SqlCommand(qry, conn);
            cmd.Parameters.Add(new SqlParameter("@dir", dir));
            if (!pathId.IsNull)
            {
                pathId = HierarchyBuilder.NewChildHierarchyId(pathId);
                CreateDirectoryQry = "INSERT INTO {0} (name, is_directory, path_locator)"
                    + " OUTPUT Inserted.path_locator"
                    + " VALUES (@dir, 1, @pathId)";
                cmd.CommandText = string.Format(CreateDirectoryQry, table);
                var param1 = new SqlParameter("@pathId", pathId) { UdtTypeName = Constants.HierarchyId };
                cmd.Parameters.Add(param1);
            }
            pathId = (SqlHierarchyId)(cmd.ExecuteScalar() ?? SqlHierarchyId.Null);
            if (pipeToOutput)
                PipeFile(table, pathId, conn);
            return pathId;
        }
        
        public virtual SqlHierarchyId FindPath(string table, string path, bool isDirectory, SqlConnection conn, bool pipeToOutput = false)
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
                return new SqlHierarchyId();
            }
            var relativePath = path.GetRelativePath(tableRoot);
            if (path.Length == relativePath.Length)
                relativePath = path.GetRelativePath(tableRootFqdn);
            var dirs = relativePath.SplitByDirectory();
            SqlHierarchyId pathLocator = SqlHierarchyId.Null;
            foreach (var dir in dirs)
            {
                var qry1 = string.Format(GetPathLocatorQry, table, pathLocator.IsNull ? "is null" : "= @pathLocator");
                SqlCommand cmd1 = new SqlCommand(qry1, conn);
                if (!pathLocator.IsNull)
                {
                    var pathLocatorParam = new SqlParameter("@pathLocator", pathLocator) { UdtTypeName = Constants.HierarchyId };
                    cmd1.Parameters.Add(pathLocatorParam);
                }
                cmd1.Parameters.Add(new SqlParameter("@dir", dir));
                pathLocator = (SqlHierarchyId)(cmd1.ExecuteScalar() ?? SqlHierarchyId.Null);
                if (pathLocator.IsNull)
                    break;
            }
            if (pipeToOutput)
                PipeFile(table, pathLocator, conn);
            return pathLocator;
        }

        public IEnumerable<File> ListFiles(string table, string directory, SqlConnection conn, bool recursive, bool excludeData, bool excludeDirectories = true)
        {
            SqlHierarchyId dirId = FindPath(table, directory, true, conn);
            string qry = BuildGetFilesQuery(table, recursive, excludeData, excludeDirectories);
            SqlCommand cmd1 = new SqlCommand(qry, conn);
            var param1 = new SqlParameter("@dirId", dirId) { UdtTypeName = Constants.HierarchyId };
            cmd1.Parameters.Add(param1);
            using (var reader = cmd1.ExecuteReader())
            {
                return reader.ToFiles();
            }
        }

        internal string BuildGetFilesQuery(string table, bool recursive, bool excludeData, bool excludeDirectories)
        {
            string qry = recursive ? ListFilesRecursiveQuery : ListFilesQuery;
            qry = string.Format(qry, table, excludeData ? " null as " : "");
            if (excludeDirectories)
                qry += ExcludeDirectories;
            return qry;
        }

        internal bool IsTableRoot(string path, string tableRoot, string tableRootFqdn)
        {
            var tableRootRelative = Path.GetFileName(tableRoot);
            return string.IsNullOrEmpty(path)
                || path.Equals(tableRoot, StringComparison.InvariantCultureIgnoreCase)
                || path.Equals(tableRootFqdn, StringComparison.InvariantCultureIgnoreCase)
                || path.TrimStart('\\').Equals(tableRootRelative, StringComparison.InvariantCultureIgnoreCase);
        }
        
        internal static void PipeFile(string table, SqlHierarchyId pathId, SqlConnection conn)
        {
            var returnQuery = string.Format("SELECT * FROM {0} WHERE path_locator = @pathId", table);
            SqlCommand cmd1 = new SqlCommand(returnQuery, conn);
            var pathIdParam = new SqlParameter("@pathId", pathId) { UdtTypeName = Constants.HierarchyId };
            cmd1.Parameters.Add(pathIdParam);
            var dataReader = cmd1.ExecuteReader();
            SqlContext.Pipe.Send(dataReader);
            dataReader.Close();
            dataReader.Dispose();
        }
        
        private static void PipeScalar(SqlDbType dbType, object scalarObj)
        {
            SqlDataRecord record = new SqlDataRecord(new SqlMetaData("stream_id", dbType));
            record.SetValues(scalarObj);
            SqlContext.Pipe.Send(record);           
        }

        public int DeleteFile(string table, Guid stream_id, SqlConnection conn)
        {
            SqlConnManager.IsConnected(conn);
            if (!FileTableExists(table, conn)) // This is used to prevent SQL injection
            {
                throw new Exception("Table does not exists or is not a FileTable.");
            }
            var deleteQry = "DELETE FROM {0} WHERE stream_id = @id";
            var qry = string.Format(deleteQry, table);
            var cmd = new SqlCommand(qry, conn);
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.UniqueIdentifier) { Value = stream_id });
            return cmd.ExecuteNonQuery();
        }

        public int DeleteFile(string table, SqlHierarchyId hierarchyid, SqlConnection conn)
        {
            SqlConnManager.IsConnected(conn);
            if (!FileTableExists(table, conn)) // This is used to prevent SQL injection
            {
                throw new Exception("Table does not exists or is not a FileTable.");
            }
            var deleteQry = "DELETE FROM {0} WHERE path_locator = @hierarchyid";
            var qry = string.Format(deleteQry, table);
            var cmd = new SqlCommand(qry, conn);
            cmd.Parameters.Add(new SqlParameter("@hierarchyid", hierarchyid) { UdtTypeName = Constants.HierarchyId });
            return cmd.ExecuteNonQuery();
        }

        public int RenameFile(string table, Guid stream_id, string filename, SqlConnection conn)
        {
            SqlConnManager.IsConnected(conn);
            if (!FileTableExists(table, conn)) // This is used to prevent SQL injection
            {
                throw new Exception("Table does not exists or is not a FileTable.");
            }
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename");
            }
            var insertQry = "UPDATE {0} SET Name = @fileName WHERE stream_id = @id";
            var qry = string.Format(insertQry, table);
            var cmd = new SqlCommand(qry, conn);
            cmd.Parameters.Add(new SqlParameter("@fileName", filename));
            cmd.Parameters.Add(new SqlParameter("@id", stream_id));
            return cmd.ExecuteNonQuery();
        }

        public int RenameFile(string table, SqlHierarchyId hierarchyid, string filename, SqlConnection conn)
        {
            SqlConnManager.IsConnected(conn);
            if (!FileTableExists(table, conn)) // This is used to prevent SQL injection
            {
                throw new Exception("Table does not exists or is not a FileTable.");
            }
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException("filename");
            }
            var insertQry = "UPDATE {0} SET Name = @fileName WHERE path_locator = @hierarchyId";
            var qry = string.Format(insertQry, table);
            var cmd = new SqlCommand(qry, conn);
            cmd.Parameters.Add(new SqlParameter("@fileName", filename));
            cmd.Parameters.Add(new SqlParameter("@hierarchyId", hierarchyid) { UdtTypeName = Constants.HierarchyId });
            return cmd.ExecuteNonQuery();
        }

        private const string GetPathLocatorQry = "SELECT path_locator FROM {0} WHERE parent_path_locator {1} AND name = @dir";
        private const string ListFilesQuery = "SELECT [stream_id],{1}[file_stream],[name],[path_locator],[parent_path_locator],[file_type],[cached_file_size],[creation_time],[last_write_time],[last_access_time],[is_directory],[is_offline],[is_hidden],[is_readonly],[is_archive],[is_system],[is_temporary] FROM {0} WHERE parent_path_locator = @dirId";
        private const string ListFilesRecursiveQuery = "SELECT [stream_id],{1}[file_stream],[name],[path_locator],[parent_path_locator],[file_type],[cached_file_size],[creation_time],[last_write_time],[last_access_time],[is_directory],[is_offline],[is_hidden],[is_readonly],[is_archive],[is_system],[is_temporary] FROM {0} WHERE path_locator.IsDescendantOf(@dirId) = 1";
        private const string ExcludeDirectories = " AND is_directory = 0";

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
