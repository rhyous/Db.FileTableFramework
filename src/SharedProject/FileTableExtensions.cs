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
        #region Query files
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
            return (ftManager ?? new FileTableManager()).DirectoryExists(table, path, conn);
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
            return (ftManager ?? new FileTableManager()).FileExists(table, path, conn);
        }

        /// <summary>
        /// Lists files in a directory.
        /// </summary>
        /// <param name="table">The FileTable.</param>
        /// <param name="path">The directory path.</param>
        /// <param name="excludeDirectories">Directories to exclude.</param>
        /// <returns></returns>
        [SqlFunction(DataAccess = DataAccessKind.Read, SystemDataAccess = SystemDataAccessKind.Read,
                    FillRowMethodName = "FillFileRow",
                    TableDefinition = "stream_id uniqueidentifier, file_stream varbinary(max)")]
        public static IEnumerable GetFilesInDirectory(string table, string path, bool recursive, bool excludeData, bool excludeDirectories = true)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                return _GetFilesInDirectory(table, path, recursive, excludeDirectories, excludeData, conn);
            }
        }

        /// <summary>
        /// Used for unit testing to inject a separate rep.
        /// </summary>
        internal static IEnumerable<File> _GetFilesInDirectory(string table, string directory, bool recursive, bool excludeData, bool excludeDirectories, SqlConnection conn, IFileTableManager ftManager = null)
        {
            return (ftManager ?? new FileTableManager()).GetFilesInDirectory(table, directory, conn, recursive, excludeData, excludeDirectories);
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
        #endregion

        #region Create directory
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
            (ftManager ?? new FileTableManager()).CreateDirectory(table, path, conn, true);
        }
        #endregion

        #region Create file
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
            return (ftManager ?? new FileTableManager()).CreateFile(table, path, data, conn);
        }

        #endregion

        #region Delete file
        /// <summary>
        /// Deletes a file by path.
        /// </summary>
        /// <param name="table">The specified FileTable</param>
        /// <param name="path">The path to the file.</param>
        /// <returns></returns>
        [SqlProcedure()]
        public static int DeleteFileByPath(string table, string path)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                var ftManager = new FileTableManager();
                var hierarchyId = ftManager.FileExists(table, path, conn);
                return _DeleteFileByPathLocator(table, hierarchyId, conn, ftManager);
            }
        }

        /// <summary>
        /// Deletes a file by stream_id, which is a Guid.
        /// </summary>
        /// <param name="table">The specified FileTable</param>
        /// <param name="stream_id">The id to the file.</param>
        /// <returns></returns>
        [SqlProcedure()]
        public static int DeleteFileByStreamId(string table, Guid stream_id)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                return _DeleteFileByStreamId(table, stream_id, conn);
            }
        }

        /// <summary>
        /// Used for unit testing to inject a separate rep.
        /// </summary>
        internal static int _DeleteFileByStreamId(string table, Guid stream_id, SqlConnection conn, IFileTableManager ftManager = null)
        {
            return (ftManager ?? new FileTableManager()).DeleteFile(table, stream_id, conn);
        }

        /// <summary>
        /// Deletes a file by path_locator, which is a SqlHierarchyId.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="hierarchyId"></param>
        /// <returns></returns>
        [SqlProcedure()]
        public static int DeleteFileByPathLocator(string table, SqlHierarchyId path_locator)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                return _DeleteFileByPathLocator(table, path_locator, conn);
            }
        }

        /// <summary>
        /// Used for unit testing to inject a separate rep.
        /// </summary>
        internal static int _DeleteFileByPathLocator(string table, SqlHierarchyId hierarchyId, SqlConnection conn, IFileTableManager ftManager = null)
        {
            return (ftManager ?? new FileTableManager()).DeleteFile(table, hierarchyId, conn);
        }

        #endregion

        #region Rename file
        /// <summary>
        /// Renames a file by path.
        /// </summary>
        /// <param name="table">The specified FileTable</param>
        /// <param name="path">The path to the file.</param>
        /// <returns></returns>
        [SqlProcedure()]
        public static int RenameFileByPath(string table, string path, string newFilename)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                var ftManager = new FileTableManager();
                var hierarchyId = ftManager.FileExists(table, path, conn);
                return _RenameFileByPathLocator(table, hierarchyId, newFilename, conn, ftManager);
            }
        }

        /// <summary>
        /// Renames a file by stream_id, which is a Guid.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="stream_id"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [SqlProcedure()]
        public static int RenameFileByStreamId(string table, Guid stream_id, string newFilename)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                return _RenameFileByStreamId(table, stream_id, newFilename, conn);
            }
        }

        /// <summary>
        /// Used for unit testing to inject a separate rep.
        /// </summary>
        internal static int _RenameFileByStreamId(string table, Guid hierarchyId, string newFilename, SqlConnection conn, IFileTableManager ftManager = null)
        {
            return (ftManager ?? new FileTableManager()).RenameFile(table, hierarchyId, newFilename, conn);
        }

        /// <summary>
        /// Renames a file by path_locator, which is a SqlHierarchyId.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="hierarchyId"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [SqlProcedure()]
        public static int RenameFileByPathLocator(string table, SqlHierarchyId path_locator, string newFilename)
        {
            using (SqlConnection conn = new SqlConnection("context connection=true"))
            {
                conn.Open();
                return _RenameFileByPathLocator(table, path_locator, newFilename, conn);
            }
        }

        /// <summary>
        /// Used for unit testing to inject a separate rep.
        /// </summary>
        internal static int _RenameFileByPathLocator(string table, SqlHierarchyId hierarchyId, string newFilename, SqlConnection conn, IFileTableManager ftManager = null)
        {
            return (ftManager ?? new FileTableManager()).RenameFile(table, hierarchyId, newFilename, conn);
        }
        #endregion
    }
}