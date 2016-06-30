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
            var dirManager = new DirectoryManager();
            var fileTableRepo = new Mock<FileTableRepo>();
            fileTableRepo.Setup(m => m.FindDirectory(table, It.IsAny<string>(), It.IsAny<SqlConnection>())).Returns<string>(null);
            fileTableRepo.Setup(m => m.CreateDirectory(table, "dir1", It.IsAny<string>(), It.IsAny<SqlConnection>())).Returns("ABC12301");
            fileTableRepo.Setup(m => m.CreateDirectory(table, "dir2", It.IsAny<string>(), It.IsAny<SqlConnection>())).Returns("ABC12302");
            fileTableRepo.Setup(m => m.CreateDirectory(table, "dir3", It.IsAny<string>(), It.IsAny<SqlConnection>())).Returns("ABC12303");
            fileTableRepo.Setup(m => m.GetTableRootPath(table, null)).Returns(tableRoot);
            dirManager.FileTableRepo = fileTableRepo.Object;

            // Act
            var pathId = dirManager.CreateDirectory(table, dirStructure, null);

            // Assert
            Assert.AreEqual("ABC12303", pathId);
        }

        [TestMethod]
        public void CreateDirectoryAlreadyExistsTest()
        {
            // Arrange
            var table = "MyTable";
            var tableRoot = @"\MyTableDir";
            var dirStructure = @"dir1\dir2\dir3";
            var dirManager = new DirectoryManager();
            var fileTableRepo = new Mock<FileTableRepo>();
            fileTableRepo.Setup(m => m.FindDirectory(table, dirStructure, It.IsAny<SqlConnection>())).Returns("ABC12303");
            fileTableRepo.Setup(m => m.GetTableRootPath(table, null)).Returns(tableRoot);
            dirManager.FileTableRepo = fileTableRepo.Object;

            // Act
            var pathId = dirManager.CreateDirectory(table, dirStructure, null);

            // Assert
            Assert.AreEqual("ABC12303", pathId);
        }

        [TestMethod]
        public void DirectoryExistsNotFoundTest()
        {
            // Arrange
            var table = "MyTable";
            var tableRoot = @"\MyTableDir";
            var dirStructure = @"dir1\dir2\dir3";
            var dirManager = new DirectoryManager();
            var fileTableRepo = new Mock<FileTableRepo>();
            fileTableRepo.Setup(m => m.FindDirectory(table, It.IsAny<string>(), It.IsAny<SqlConnection>())).Returns<string>(null);
            dirManager.FileTableRepo = fileTableRepo.Object;

            fileTableRepo.Setup(m => m.FileTableExists(table, null)).Returns(true);
            fileTableRepo.Setup(m => m.GetTableRootPath(table, null)).Returns(tableRoot);

            // Act
            var pathId = dirManager.DirectoryExists(table, dirStructure, null);

            // Assert
            Assert.IsNull(pathId);
        }

        [TestMethod]
        public void DirectoryExistsFoundTest()
        {
            // Arrange
            var table = "MyTable";
            var tableRoot = @"\MyTableDir";
            var dirStructure = @"dir1\dir2\dir3";
            var id = "12345679";
            var dirManager = new DirectoryManager();
            var fileTableRepo = new Mock<FileTableRepo>();
            fileTableRepo.Setup(m => m.FindDirectory(It.IsAny<string>(), It.IsAny<string>(), null)).Returns(id);
            dirManager.FileTableRepo = fileTableRepo.Object;

            var fileTableManagerMock = new Mock<IFileTableRepo>();
            fileTableManagerMock.Setup(m => m.FileTableExists(table, null)).Returns(true);
            fileTableManagerMock.Setup(m => m.GetTableRootPath(table, null)).Returns(tableRoot);

            // Act
            var pathId = dirManager.DirectoryExists(table, dirStructure, null);

            // Assert
            Assert.AreEqual(id, pathId);
        }
    }
}
