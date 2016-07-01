﻿using Rhyous.Db.FileTableFramework.Interfaces;
using Rhyous.Db.FileTableFramework.Managers;
using System;
using System.Data.SqlClient;
using System.IO;

namespace Rhyous.Db.FileTableFramework.Repos
{
    internal class FileTableRepo : IFileTableRepo
    {
        public string CreateFile(string table, string fileName, string pathId, byte[] data, SqlConnection conn)
        {
            SqlConnManager.IsConnected(conn);
            if (!FileTableExists(table, conn))
            {
                throw new Exception("Table does not exists or is not a FileTable.");
            }
            if (string.IsNullOrWhiteSpace(pathId))
            {
                pathId = NewChildHierarchyId(null);
            }
            var insertQry = "INSERT INTO {0} (name, file_stream, path_locator) "
                          + " OUTPUT Inserted.path_locator.ToString()"
                          + " VALUES (@fileName, @data, HierarchyId::Parse(@pathId))";
            var qry = string.Format(insertQry, table);
            var cmd = new SqlCommand(qry, conn);
            cmd.Parameters.Add(new SqlParameter("@fileName", fileName));
            cmd.Parameters.Add(new SqlParameter("@data", data));
            cmd.Parameters.Add(new SqlParameter("@pathId", pathId));
            var hierarchyid = cmd.ExecuteScalar() as string;
            return hierarchyid;
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
                pathId = NewChildHierarchyId(pathId);
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
            var tableDir = Path.GetFileName(tableRoot);
            var tableDirAndPath = @"\" + Path.Combine(tableDir, path.Trim(@"\".ToCharArray()));
            var qry = string.Format(PathExistsQry, table);
            SqlCommand cmd = new SqlCommand(qry, conn);
            cmd.Parameters.Add(new SqlParameter("@path", path));
            cmd.Parameters.Add(new SqlParameter("@tableDirAndPath", tableDirAndPath));
            cmd.Parameters.Add(new SqlParameter("@isDir", isDirectory));
            var hierarchyId = cmd.ExecuteScalar() as string;
            return hierarchyId;
        }

        internal bool IsTableRoot(string path, string tableRoot, string tableRootFqdn)
        {
            var tableRootRelative = Path.GetFileName(tableRoot);
            return path.Equals(tableRoot, StringComparison.InvariantCultureIgnoreCase)
                || path.Equals(tableRootFqdn, StringComparison.InvariantCultureIgnoreCase)
                || path.TrimStart('\\').Equals(tableRootRelative, StringComparison.InvariantCultureIgnoreCase);
        }

        public string NewChildHierarchyId(string pathId)
        {
            return NewChildHierarchyId(pathId, Guid.NewGuid());
        }

        internal string NewChildHierarchyId(string pathId, Guid guid)
        {
            var template = "{0}{1}.{2}.{3}/";
            var bytes = guid.ToByteArray();
            if (string.IsNullOrWhiteSpace(pathId))
                pathId = "/"; //Root
            var hierarchyId = string.Format(template, pathId, GetLong(0, 6, bytes), GetLong(6, 6, bytes), GetLong(12, 4, bytes));
            return hierarchyId;
        }

        internal static long GetLong(int a, int b, byte[] bytes)
        {
            var subBytes = new byte[8];
            Array.Copy(bytes, a, subBytes, 8 - b, b);
            Array.Reverse(subBytes);
            return BitConverter.ToInt64(subBytes, 0);
        }

        private const string PathExistsQry = "SELECT Path_Locator.ToString() FROM {0}"
        + " WHERE file_stream.GetFileNamespacePath() = @path" // An exact match: \MyFileTable\MyPath
        + " OR file_stream.GetFileNamespacePath() = '\\' + @path" // An exact match without leading slash: MyFileTable\MyPath
        + " OR file_stream.GetFileNamespacePath() = @tableDirAndPath" // A subdir (with or without leading slash): MyPath or \MyPath
        + " OR file_stream.GetFileNamespacePath(1) = @path" // A full path: \\server\instanceDir\dbDir\MyFileTable\MyPath
        + " OR file_stream.GetFileNamespacePath(1, 2) = @path" // A full path with FQDN: \\server.domain.tld\instanceDir\dbDir\MyFileTable\MyPath
        + " AND is_directory = @isDir";



        #region Dependency Injectable Properties

        public virtual ISqlConnectionManager SqlConnManager
        {
            get { return _SqlConnManager ?? (_SqlConnManager = new SqlConnectionManager()); }
            set { _SqlConnManager = value; }
        }
        private ISqlConnectionManager _SqlConnManager;

        #endregion
    }
}
