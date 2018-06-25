using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Rhyous.Db.FileTableFramework.Interfaces
{
    public interface IFileTableRepo
    {

        /// <summary>
        /// Creates a directory at the specified location using the location HierachyId
        /// as a string. If hierarchyid is null, empty, or whitespace, the directory is
        /// added to the root.
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="fileName">The name of the file to create. It should not include
        /// a path.</param>
        /// <param name="hierarchyid">The string representation of the HierarchyId, which
        /// is the path_locator column.</param>
        /// <param name="data">The file data</param>
        /// <param name="conn">The Sql Connection string</param>
        /// <returns></returns>
        Guid CreateFile(string table, string fileName, SqlHierarchyId hierarchyid, byte[] data, SqlConnection conn);

        /// <summary>
        /// Creates a directory at the specified location using the location HierachyId
        /// as a string. If hierarchyid is null, empty, or whitespace, the directory is
        /// added to the root.
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="dir">The directory to create</param>
        /// <param name="hierarchyid">The string representation of the HierarchyId, which
        /// is the path_locator column.</param>
        /// <param name="conn">The Sql Connection string</param>
        /// <returns></returns>
        SqlHierarchyId CreateDirectory(string table, string dir, SqlHierarchyId hierarchyid, SqlConnection conn, bool pipeToOutput = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="path"></param
        /// <param name="parentPathId"></param>
        /// <param name="tableDirAndPath"></param>
        /// <param name="conn"></param>
        /// <param name="pipeToOutput"></param>
        /// <returns></returns>
        SqlHierarchyId FindPath(string table, string path, bool isDirectory, SqlConnection conn, bool pipeToOutput = false);

        /// <summary>
        /// Checks if the FileTable exists.
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="conn">The Sql Connection string</param>
        /// <returns></returns>
        bool FileTableExists(string table, SqlConnection conn);

        /// <summary>
        /// Gets the root path of a FileTable
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="conn">The Sql Connection string</param>
        /// <returns></returns>
        string GetTableRootPath(string table, int option, SqlConnection conn);

        /// <summary>
        /// Outputs the list of files in a directory.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="directory"></param>
        /// <param name="conn"></param>
        /// <param name="excludeDirectories">Incldue directories or not.</param>
        /// <returns></returns>
        IEnumerable<File> GetFilesInDirectory(string table, string directory, SqlConnection conn, bool excludeDirectories);

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
        /// <param name="conn"></param>
        /// <returns>The number of deleted rows.</returns>
        int DeleteFile(string table, SqlHierarchyId hierarchyid, SqlConnection conn);

        /// <summary>
        /// Deletes a file by id, which is a Guid.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="stream_id"></param>
        /// <param name="filename"></param>
        /// <param name="conn"></param>
        /// <returns>True if renamed.</returns>   
        int RenameFile(string table, Guid stream_id, string filename, SqlConnection conn);

        /// <summary>
        /// Deletes a file by path_locator, which is a SqlHierarchyId.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="hierarchyid"></param>
        /// <param name="filename"></param>
        /// <param name="conn"></param>
        /// <returns>True if renamed.</returns>
        int RenameFile(string table, SqlHierarchyId hierarchyid, string filename, SqlConnection conn);
    }
}