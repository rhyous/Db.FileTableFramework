using Microsoft.SqlServer.Types;
using Rhyous.Db.FileTableFramework.Business;
using Rhyous.Db.FileTableFramework.Interfaces;
using Rhyous.Db.FileTableFramework.Repos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace Rhyous.Db.FileTableFramework.Managers
{
    internal class FileTableManager : IFileTableManager
    {
        public Guid CreateFile(string table, string path, byte[] data, SqlConnection conn)
        {
            var file = Path.GetFileName(path);
            var dir = Path.GetDirectoryName(path);
            var pathId = DirectoryExists(table, dir, conn);
            if (pathId.IsNull && !string.IsNullOrWhiteSpace(dir))
                pathId = CreateDirectory(table, dir, conn, false);
            var hierarchyId = FileTableRepo.CreateFile(table, file, HierarchyBuilder.NewChildHierarchyId(pathId), data, conn);
            return hierarchyId;
        }

        public virtual SqlHierarchyId CreateDirectory(string table, string path, SqlConnection conn, bool pipeToOutput)
        {
            SqlHierarchyId pathId;
            var tmpPath = path.Trim('\\');
            var dirsToCreate = new Stack<string>();
            pathId = SqlHierarchyId.Null;
            while (pathId.IsNull && !string.IsNullOrWhiteSpace(tmpPath.Trim('\\')))
            {
                pathId = DirectoryExists(table, tmpPath, conn);
                if (pathId.IsNull)
                {
                    dirsToCreate.Push(Path.GetFileName(tmpPath));
                    tmpPath = Path.GetDirectoryName(tmpPath);
                }
            }
            if (dirsToCreate.Count > 0)
            {
                pathId = CreateDirectoryStructure(table, dirsToCreate, pathId, conn, pipeToOutput);
            }
            return pathId;
        }

        public virtual SqlHierarchyId CreateDirectoryStructure(string table, Stack<string> dirsToCreate, SqlHierarchyId pathId, SqlConnection conn, bool pipeToOutput)
        {
            while (dirsToCreate.Count > 0)
            {
                var dir = dirsToCreate.Pop();
                pathId = FileTableRepo.CreateDirectory(table, dir, pathId, conn, pipeToOutput);
            }
            return pathId;
        }

        public virtual SqlHierarchyId DirectoryExists(string table, string path, SqlConnection conn)
        {
            return FileTableRepo.FindPath(table, path, true, conn);
        }

        public virtual SqlHierarchyId FileExists(string table, string path, SqlConnection conn)
        {
            return FileTableRepo.FindPath(table, path, false, conn);
        }

        public IEnumerable<File> ListFiles(string table, string directory, SqlConnection conn, bool recursive, bool excludeData, bool excludeDirectories)
        {
            return FileTableRepo.ListFiles(table, directory, conn, recursive, excludeData, excludeDirectories);
        }

        public int DeleteFile(string table, Guid stream_id, SqlConnection conn)
        {
            return FileTableRepo.DeleteFile(table, stream_id, conn);
        }

        public int DeleteFile(string table, SqlHierarchyId hierarchyid, SqlConnection conn)
        {
            return FileTableRepo.DeleteFile(table, hierarchyid, conn);
        }

        public int RenameFile(string table, Guid stream_id, string filename, SqlConnection conn)
        {
            return FileTableRepo.RenameFile(table, stream_id, filename, conn);
        }

        public int RenameFile(string table, SqlHierarchyId hierarchyid, string filename, SqlConnection conn)
        {
            return FileTableRepo.RenameFile(table, hierarchyid, filename, conn);
        }

        #region Dependency Injectable Properties

        public IFileTableRepo FileTableRepo
        {
            get { return _DirRepo ?? (_DirRepo = new FileTableRepo()); }
            set { _DirRepo = value; }
        }
        private IFileTableRepo _DirRepo;

        public IHierarchyBuilder HierarchyBuilder
        {
            get { return _HierarchyBuilder ?? (_HierarchyBuilder = new HierarchyBuilder()); }
            set { _HierarchyBuilder = value; }
        }
        private IHierarchyBuilder _HierarchyBuilder;
        #endregion

    }
}
