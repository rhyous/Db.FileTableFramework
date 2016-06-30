using Rhyous.Db.FileTableFramework.Interfaces;
using Rhyous.Db.FileTableFramework.Repos;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace Rhyous.Db.FileTableFramework.Managers
{
    class DirectoryManager : IFileTableManager
    {
        public string CreateFile(string table, string path, byte[] data, SqlConnection conn)
        {
            var file = Path.GetFileName(path);
            var dir = Path.GetDirectoryName(path);
            var pathId = DirectoryExists(table, dir, conn);
            if (string.IsNullOrWhiteSpace(pathId))
                pathId = CreateDirectory(table, path, conn);
            var hierarchyId = FileTableRepo.CreateFile(table, file, FileTableRepo.NewChildHierarchyId(pathId), data, conn);
            return hierarchyId;
        }

        public virtual string CreateDirectory(string table, string path, SqlConnection conn)
        {
            string pathId;
            var tmpPath = path.TrimEnd('\\');
            var dirsToCreate = new Stack<string>();
            pathId = null;
            while (string.IsNullOrWhiteSpace(pathId) && !string.IsNullOrWhiteSpace(tmpPath))
            {
                pathId = DirectoryExists(table, tmpPath, conn);
                if (string.IsNullOrWhiteSpace(pathId))
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

        public virtual string CreateDirectoryStructure(string table, Stack<string> dirsToCreate, string pathId, SqlConnection conn)
        {
            while (dirsToCreate.Count > 0)
            {
                pathId = FileTableRepo.CreateDirectory(table, dirsToCreate.Pop(), pathId, conn);
            }
            return pathId;
        }

        public virtual string DirectoryExists(string table, string path, SqlConnection conn)
        {
            return FileTableRepo.FindDirectory(table, path, conn);
        }

        #region Dependency Injectable Properties

        public IFileTableRepo FileTableRepo
        {
            get { return _DirRepo ?? (_DirRepo = new FileTableRepo()); }
            set { _DirRepo = value; }
        }
        private IFileTableRepo _DirRepo;

        #endregion

    }
}
