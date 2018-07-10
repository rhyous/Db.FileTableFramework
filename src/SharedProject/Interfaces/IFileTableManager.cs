using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Rhyous.Db.FileTableFramework.Interfaces
{
    public interface IFileTableManager
    {
        /// <summary>
        /// Checks for the existance of a directory in a FileTable
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="path">The path of the directory. It must be a full path,
        /// or a path relative from, but not including, the table's root directory.</param>
        /// <param name="conn">The Sql Connection string</param>
        /// <returns>The directory's HierarchyId if it exists, null if it doesn't.</returns>
        SqlHierarchyId DirectoryExists(string table, string path, SqlConnection conn);

        /// <summary>
        /// Checks for the existance of a file in a FileTable
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="path">The path of the directory. It must be a full path,
        /// or a path relative from, but not including, the table's root directory.</param>
        /// <param name="conn">The Sql Connection string</param>
        /// <returns>The files's HierarchyId if it exists, null if it doesn't.</returns>
        SqlHierarchyId FileExists(string table, string path, SqlConnection conn);

        /// <summary>
        /// Creates a directory in a FileTable. If it already exists, it must
        /// not error, but instead return the pathId.
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="path">The path to create. It should create the full path,
        /// even if multiple directory levels need to be create: \a\b\c
        /// </param>
        /// <param name="conn">The Sql Connection string</param>
        /// <param name="pipeToOutput">Whether to pipe the output or not.</param>
        /// <returns>The directory's HierarchyId.</returns>
        SqlHierarchyId CreateDirectory(string table, string path, SqlConnection conn, bool pipeToOutput);

        /// <summary>
        /// Creates A directory structure, such as: a\b\c
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="dirsToCreate">A stack of directories to create.</param>
        /// <param name="pathId">The path_locator or HierarchyId of the starting directory.
        /// <param name="conn"></param>
        /// <param name="pipeToOutput">Whether to pipe the output or not.</param>
        /// <returns></returns>
        SqlHierarchyId CreateDirectoryStructure(string table, Stack<string> dirsToCreate, SqlHierarchyId pathId, SqlConnection conn, bool pipeToOutput);
        
        /// <summary>
        /// Adds a file to a FileTable. Allows for specifying a path.
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="path">The path of the file. It must be a full path,
        /// or a path relative from, but not including, the table's root directory.</param>
        /// <param name="data">The file data as a byte array.</param>
        /// <param name="conn">The Sql Connection string.</param>
        /// <returns></returns>
        Guid CreateFile(string table, string path, byte[] data, SqlConnection conn);

        /// <summary>
        /// Lists all the files in a directory. The directory should be the path from the root of the table.
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="directory">The path of the file. It must be a full directory path,
        /// or a path relative from, but not including, the table's root directory.</param>
        /// <param name="conn">The Sql Connection string.</param>
        /// <param name="recursive">If true, gets all files even if under a subfolder.</param>
        /// <param name="excludeDirectories">Whether to exclude directories.</param>
        /// <returns></returns>
        IEnumerable<File> GetFilesInDirectory(string table, string directory, SqlConnection conn, bool recursive, bool excludeData, bool excludeDirectories);

        /// <summary>
        /// Deletes a file by id, which is a Guid.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="stream_id"></param>
        /// <returns>The number of deleted rows.</returns>        
        int DeleteFile(string table, Guid stream_id, SqlConnection conn);

        /// <summary>
        /// Deletes a file by path_locator, which is a SqlHierarchyId.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="hierarchyid"></param>
        /// <returns>The number of deleted rows.</returns>
        int DeleteFile(string table, SqlHierarchyId hierarchyid, SqlConnection conn);
        /// <summary>
        /// Deletes a file by id, which is a Guid.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="stream_id"></param>
        /// <param name="filename"></param>
        /// <returns>The number of deleted rows.</returns>        
        int RenameFile(string table, Guid stream_id, string filename, SqlConnection conn);

        /// <summary>
        /// Deletes a file by path_locator, which is a SqlHierarchyId.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="hierarchyid"></param>
        /// <param name="filename"></param>
        /// <returns>The number of deleted rows.</returns>
        int RenameFile(string table, SqlHierarchyId hierarchyid, string filename, SqlConnection conn);
    }
}
