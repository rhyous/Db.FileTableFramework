using Microsoft.SqlServer.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.Db.FileTableFramework.Interfaces;
using Rhyous.Db.FileTableFramework.Managers;
using Rhyous.Db.FileTableFramework.Repos;
using System.Data.SqlClient;
using System.Text;

namespace Rhyous.Db.FileTableFramework.Tests.Managers
{
    [TestClass]
    public class DirectoryManagerTests
    {

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2}", b);
            return hex.ToString();
        }


        [TestMethod]
        public void CreateDirectoryTest()
        {
            // Arrange
            var table = "MyTable";
            var tableRoot = @"\MyTableDir";
            var dirStructure = @"dir1\dir2\dir3";
            var dirManager = new FileTableManager();
            var fileTableRepo = new Mock<FileTableRepo>();
            var id1 = SqlHierarchyId.Parse("/0/");
            var id2 = SqlHierarchyId.Parse("/0/1/");
            var id3 = SqlHierarchyId.Parse("/0/1/2/");
            fileTableRepo.Setup(m => m.FindPath(table, It.IsAny<string>(), true, It.IsAny<SqlConnection>(), false)).Returns<string>(null);
            fileTableRepo.Setup(m => m.CreateDirectory(table, "dir1", It.IsAny<SqlHierarchyId>(), It.IsAny<SqlConnection>(), false)).Returns(id1);
            fileTableRepo.Setup(m => m.CreateDirectory(table, "dir2", It.IsAny<SqlHierarchyId>(), It.IsAny<SqlConnection>(), false)).Returns(id2);
            fileTableRepo.Setup(m => m.CreateDirectory(table, "dir3", It.IsAny<SqlHierarchyId>(), It.IsAny<SqlConnection>(), true)).Returns(id3);
            fileTableRepo.Setup(m => m.GetTableRootPath(table, 0, null)).Returns(tableRoot);
            dirManager.FileTableRepo = fileTableRepo.Object;

            // Act
            var pathId = dirManager.CreateDirectory(table, dirStructure, null);

            // Assert
            Assert.AreEqual(id3, pathId);
        }

        [TestMethod]
        public void CreateDirectoryAlreadyExistsTest()
        {
            // Arrange
            var table = "MyTable";
            var tableRoot = @"\MyTableDir";
            var dirStructure = @"dir1\dir2\dir3";
            var dirManager = new FileTableManager();
            var fileTableRepo = new Mock<FileTableRepo>();
            var stringId = "/0/";
            var id = SqlHierarchyId.Parse(stringId);
            fileTableRepo.Setup(m => m.FindPath(table, dirStructure, true, It.IsAny<SqlConnection>(), false)).Returns(id);
            fileTableRepo.Setup(m => m.GetTableRootPath(table, 0, null)).Returns(tableRoot);
            dirManager.FileTableRepo = fileTableRepo.Object;

            // Act
            var pathId = dirManager.CreateDirectory(table, dirStructure, null);

            // Assert
            Assert.AreEqual(stringId, pathId.ToString());
        }

        [TestMethod]
        public void DirectoryExistsNotFoundTest()
        {
            // Arrange
            var table = "MyTable";
            var tableRoot = @"\MyTableDir";
            var dirStructure = @"dir1\dir2\dir3";
            var dirManager = new FileTableManager();
            var fileTableRepo = new Mock<FileTableRepo>();
            fileTableRepo.Setup(m => m.FindPath(table, It.IsAny<string>(), true, It.IsAny<SqlConnection>(), false)).Returns<string>(null);
            dirManager.FileTableRepo = fileTableRepo.Object;

            fileTableRepo.Setup(m => m.FileTableExists(table, null)).Returns(true);
            fileTableRepo.Setup(m => m.GetTableRootPath(table, 0, null)).Returns(tableRoot);

            // Act
            var pathId = dirManager.DirectoryExists(table, dirStructure, null);

            // Assert
            Assert.IsTrue(pathId.IsNull);
        }

        [TestMethod]
        public void DirectoryExistsFoundTest()
        {
            // Arrange
            var table = "MyTable";
            var tableRoot = @"\MyTableDir";
            var dirStructure = @"dir1\dir2\dir3";
            var id = SqlHierarchyId.Parse("/0/1/2/3/");
            var dirManager = new FileTableManager();
            var fileTableRepo = new Mock<FileTableRepo>();
            fileTableRepo.Setup(m => m.FindPath(It.IsAny<string>(), It.IsAny<string>(), true, null, false)).Returns(id);
            dirManager.FileTableRepo = fileTableRepo.Object;

            var fileTableManagerMock = new Mock<IFileTableRepo>();
            fileTableManagerMock.Setup(m => m.FileTableExists(table, null)).Returns(true);
            fileTableManagerMock.Setup(m => m.GetTableRootPath(table, 0, null)).Returns(tableRoot);

            // Act
            var pathId = dirManager.DirectoryExists(table, dirStructure, null);

            // Assert
            Assert.AreEqual(id, pathId);
        }

        [TestMethod]
        public void FileExistsFoundTest()
        {
            // Arrange
            var table = "MyTable";
            var tableRoot = @"\MyTableDir";
            var dirStructure = @"dir1\dir2\file.txt";
            var id = SqlHierarchyId.Parse("/1/2/3/");
            var dirManager = new FileTableManager();
            var fileTableRepo = new Mock<FileTableRepo>();
            fileTableRepo.Setup(m => m.FindPath(It.IsAny<string>(), It.IsAny<string>(), false, null, false)).Returns(id);
            dirManager.FileTableRepo = fileTableRepo.Object;

            var fileTableManagerMock = new Mock<IFileTableRepo>();
            fileTableManagerMock.Setup(m => m.FileTableExists(table, null)).Returns(true);
            fileTableManagerMock.Setup(m => m.GetTableRootPath(table, 0, null)).Returns(tableRoot);

            // Act
            var pathId = dirManager.FileExists(table, dirStructure, null);

            // Assert
            Assert.AreEqual(id, pathId);
        }
    }
}
