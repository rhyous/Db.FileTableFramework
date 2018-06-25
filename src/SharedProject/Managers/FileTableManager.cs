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
            if (pathId.IsNull)
                pathId = CreateDirectory(table, dir, conn);
            var hierarchyId = FileTableRepo.CreateFile(table, file, HierarchyBuilder.NewChildHierarchyId(pathId), data, conn);
            return hierarchyId;
        }

        public virtual SqlHierarchyId CreateDirectory(string table, string path, SqlConnection conn)
        {
            SqlHierarchyId pathId;
            var tmpPath = path.TrimEnd('\\');
            var dirsToCreate = new Stack<string>();
            pathId = SqlHierarchyId.Null;
            while (pathId.IsNull && !string.IsNullOrWhiteSpace(tmpPath))
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
                pathId = CreateDirectoryStructure(table, dirsToCreate, pathId, conn);
            }
            return pathId;
        }

        public virtual SqlHierarchyId CreateDirectoryStructure(string table, Stack<string> dirsToCreate, SqlHierarchyId pathId, SqlConnection conn)
        {
            while (dirsToCreate.Count > 0)
            {
                var dir = dirsToCreate.Pop();
                var output = dirsToCreate.Count == 0;
                pathId = FileTableRepo.CreateDirectory(table, dir, pathId, conn, output);
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

        public IEnumerable<File> GetFilesInDirectory(string table, string directory, SqlConnection conn, bool excludeDirectories)
        {
            return FileTableRepo.GetFilesInDirectory(table, directory, conn, excludeDirectories);
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
