using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using Rhyous.Db.FileTableFramework.Interfaces;
using Rhyous.Db.FileTableFramework.Managers;
using Rhyous.Db.FileTableFramework.Repos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Rhyous.Db.FileTableFramework
{
    public class FileTableExtensions
    {
        /// <summary>
        /// Checks that a FileTable exists. This is critical to prevent Sql injection.
        /// </summary>
        /// <param name="table">A file table.</param>
        /// <returns>bool</returns>
        [SqlFunction(DataAccess = DataAccessKind.Read, SystemDataAccess = SystemDataAccessKind.Read)]
        public static bool FileTableExists(string table)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                return _FileTableExists(table, conn);
            }
        }

        /// <summary>
        /// Used for unit testing to inject a separate rep.
        /// </summary>
        internal static bool _FileTableExists(string table, SqlConnection conn, IFileTableRepo repo = null)
        {
            return (repo ?? new FileTableRepo()).FileTableExists(table, conn);
        }

        /// <summary>
        /// Checks if a directory exists.
        /// </summary>
        /// <param name="table">The table to check in.</param>
        /// <param name="path">The path to check.</param>
        /// <returns></returns>
        [SqlFunction(DataAccess = DataAccessKind.Read, SystemDataAccess = SystemDataAccessKind.Read)]
        public static SqlHierarchyId DirectoryExists(string table, string path)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                return _DirectoryExists(table, path, conn);
            }
        }

        /// <summary>
        /// Used for unit testing to inject a separate rep.
        /// </summary>
        internal static SqlHierarchyId _DirectoryExists(string table, string path, SqlConnection conn, IFileTableManager ftManager = null)
        {
            return (ftManager ?? new DirectoryManager()).DirectoryExists(table, path, conn);
        }

        [SqlFunction(DataAccess = DataAccessKind.Read, SystemDataAccess = SystemDataAccessKind.Read)]
        public static SqlHierarchyId FileExists(string table, string path)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                return _FileExists(table, path, conn);
            }
        }

        /// <summary>
        /// Used for unit testing to inject a separate rep.
        /// </summary>
        internal static SqlHierarchyId _FileExists(string table, string path, SqlConnection conn, IFileTableManager ftManager = null)
        {
            return (ftManager ?? new DirectoryManager()).FileExists(table, path, conn);
        }

        [SqlProcedure()]
        public static void CreateDirectory(string table, string path)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                _CreateDirectory(table, path, conn);
            }
        }

        /// <summary>
        /// Used for unit testing to inject a separate rep.
        /// </summary>
        internal static void _CreateDirectory(string table, string path, SqlConnection conn, IFileTableManager ftManager = null)
        {
            (ftManager ?? new DirectoryManager()).CreateDirectory(table, path, conn);
        }

        /// <summary>
        /// Creates a file in the specified FileTable. If the directories in the path don't exist, the path will be created.
        /// </summary>
        /// <param name="table">The specified FileTable</param>
        /// <param name="path">The path to the file.</param>
        /// <param name="data">The data in bytes. For example, a text file with one line:
        ///     Hello, world!
        /// Would have this byte[] array:
        ///     0x48656C6C6F2C20776F726C6421</param>
        [SqlProcedure()]
        public static void CreateFile(string table, string path, byte[] data)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                _CreateFile(table, path, data, conn);
            }
        }

        [SqlProcedure()]
        public static void CreateTextFile(string table, string path, string text)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                _CreateFile(table, path, Encoding.UTF8.GetBytes(text), conn);
            }
        }

        /// <summary>
        /// Used for unit testing to inject a separate rep.
        /// </summary>
        internal static Guid _CreateFile(string table, string path, byte[] data, SqlConnection conn, IFileTableManager ftManager = null)
        {
            return (ftManager ?? new DirectoryManager()).CreateFile(table, path, data, conn);
        }

        [SqlFunction(DataAccess = DataAccessKind.Read, SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FillFileRow",
            TableDefinition = "stream_id uniqueidentifier, file_stream varbinary(max)")]
            //TableDefinition = "stream_id uniqueidentifier, file_stream varbinary(max), name nvarchar(255), path_locator hierarchyid, creation_time datetimeoffset(7), last_write_time datetimeoffset(7), last_access_time datetimeoffset(7), is_directory bit, is_offline bit, is_hidden bit, is_readonly bit, is_archive bit, is_system bit, is_temporary bit")]

        public static IEnumerable GetFilesInDirectory(string table, string path, bool excludeDirectories = true)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                return _GetFilesInDirectory(table, path, excludeDirectories, conn);
            }
        }

        /// <summary>
        /// Used for unit testing to inject a separate rep.
        /// </summary>
        internal static IEnumerable<File> _GetFilesInDirectory(string table, string directory, bool excludeDirectories, SqlConnection conn, IFileTableManager ftManager = null)
        {
            return (ftManager ?? new DirectoryManager()).GetFilesInDirectory(table, directory, conn, excludeDirectories);
        }

        public static void FillFileRow(Object obj, out Guid stream_id,
                                       out byte[] file_stream,
                                       out string name,
                                       out SqlHierarchyId path_locator,
                                       out SqlHierarchyId parent_path_locator,
                                       out string file_type,
                                       out long? cached_file_size, 
                                       out DateTimeOffset creation_time,
                                       out DateTimeOffset last_write_time,
                                       out DateTimeOffset? last_access_time,
                                       out bool is_directory,
                                       out bool is_offline,
                                       out bool is_hidden,
                                       out bool is_readonly,
                                       out bool is_archive,
                                       out bool is_system,
                                       out bool is_temporary
            )
        {
            File file = obj as File;
            file.FillFileRow(out stream_id, 
                             out file_stream, 
                             out name, 
                             out path_locator, 
                             out parent_path_locator, 
                             out file_type,
                             out cached_file_size,
                             out creation_time, 
                             out last_write_time, 
                             out last_access_time, 
                             out is_directory,
                             out is_offline, 
                             out is_hidden, 
                             out is_readonly, 
                             out is_archive, 
                             out is_system,
                             out is_temporary
                             );
        }
    }
}