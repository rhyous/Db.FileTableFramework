﻿using System.Collections.Generic;
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
        string CreateFile(string table, string fileName, string hierarchyid, byte[] data, SqlConnection conn);

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
        string CreateDirectory(string table, string dir, string hierarchyid, SqlConnection conn);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dir"></param
        /// <param name="parentPathId"></param>
        /// <param name="tableDirAndPath"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        string FindDirectory(string table, string dir, SqlConnection conn);

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
        string GetTableRootPath(string table, SqlConnection conn);

        /// <summary>
        /// Gets new HierachyId as string
        /// </summary>
        /// <param name="table">The name of the FileTable</param>
        /// <param name="conn">The Sql Connection string</param>
        /// <returns></returns>
        string NewChildHierarchyId(string hierarchyid);
    }
}