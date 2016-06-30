using System.Collections.Generic;
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
        /// <returns>The directory's HierarchyId as a string if it exists, null if it doesn't.</returns>
        string DirectoryExists(string table, string path, SqlConnection conn);

        /// <summary>
        /// Checks for the existance of a file in a FileTable
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="path">The path of the directory. It must be a full path,
        /// or a path relative from, but not including, the table's root directory.</param>
        /// <param name="conn">The Sql Connection string</param>
        /// <returns>The files's HierarchyId as a string if it exists, null if it doesn't.</returns>
        string FileExists(string table, string path, SqlConnection conn);

        /// <summary>
        /// Creates a directory in a FileTable. If it already exists, it must
        /// not error, but instead return the pathId.
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="path">The path to create. It should create the full path,
        /// even if multiple directory levels need to be create: \a\b\c
        /// </param>
        /// <param name="conn">The Sql Connection string</param>
        /// <returns>The directory's HierarchyId as a string.</returns>
        string CreateDirectory(string table, string path, SqlConnection conn);

        /// <summary>
        /// Creates A directory structure, such as: a\b\c
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="dirsToCreate">A stack of directories to create.</param>
        /// <param name="pathId">The path_locator or HierarchyId as a string of the starting directory.
        /// <param name="conn"></param>
        /// <returns></returns>
        string CreateDirectoryStructure(string table, Stack<string> dirsToCreate, string pathId, SqlConnection conn);
        
        /// <summary>
        /// Adds a file to a FileTable. Allows for specifying a path.
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="path">The path of the file. It must be a full path,
        /// or a path relative from, but not including, the table's root directory.</param>
        /// <param name="data"></param>
        /// <param name="conn">The Sql Connection string</param>
        /// <returns></returns>
        string CreateFile(string table, string path, byte[] data, SqlConnection conn);
    }
}
