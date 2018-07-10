using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhyous.Db.FileTableFramework.Repos;
using System.IO;

namespace Rhyous.Db.FileTableFramework.Tests.Repos
{
    [TestClass]
    public class FileTableRepoTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", @"Data\IsTableRootData.csv", "IsTableRootData#csv", DataAccessMethod.Sequential)]
        public void IsTableRootTest()
        {
            // Arrange
            var server = @"\\server";
            var serverFQDN = @"\\server.domain.tld";
            var inst = "instdir";
            var db = "dbdir";
            var table = "tdir";
            var tableRoot = Path.Combine(server, inst, db, table);
            var tableRootFqdn = Path.Combine(serverFQDN, inst, db, table);
            var repo = new FileTableRepo();

            var testDir = TestContext.DataRow[0].ToString();
            var expected = Convert.ToBoolean(TestContext.DataRow[1]);

            // Act
            var actual = repo.IsTableRoot(testDir,tableRoot, tableRootFqdn);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BuildGetFilesQuery_RecursiveFalse_ExcludeDataFalse_ExcludeDirectoriesFalse_Test()
        {
            // Arrange
            var repo = new FileTableRepo();
            var expected = "SELECT [stream_id],[file_stream],[name],[path_locator],[parent_path_locator],[file_type],[cached_file_size],[creation_time],[last_write_time],[last_access_time],[is_directory],[is_offline],[is_hidden],[is_readonly],[is_archive],[is_system],[is_temporary] FROM Table1 WHERE parent_path_locator = @dirId";

            // Act
            var actual = repo.BuildGetFilesQuery("Table1", false, false, false);

            // Assert
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void BuildGetFilesQuery_RecursiveFalse_ExcludeDataTrue_ExcludeDirectoriesTrue_Test()
        {
            // Arrange
            var repo = new FileTableRepo();
            var expected = "SELECT [stream_id], null as [file_stream],[name],[path_locator],[parent_path_locator],[file_type],[cached_file_size],[creation_time],[last_write_time],[last_access_time],[is_directory],[is_offline],[is_hidden],[is_readonly],[is_archive],[is_system],[is_temporary] FROM Table1 WHERE parent_path_locator = @dirId AND is_directory = 0";

            // Act
            var actual = repo.BuildGetFilesQuery("Table1", false, true, true);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BuildGetFilesQuery_RecursiveTrue_ExcludeDataFalse_ExcludeDirectoriesFalse_Test()
        {
            // Arrange
            var repo = new FileTableRepo();
            var expected = "SELECT [stream_id],[file_stream],[name],[path_locator],[parent_path_locator],[file_type],[cached_file_size],[creation_time],[last_write_time],[last_access_time],[is_directory],[is_offline],[is_hidden],[is_readonly],[is_archive],[is_system],[is_temporary] FROM Table1 WHERE path_locator.IsDescendantOf(@dirId) = 1";

            // Act
            var actual = repo.BuildGetFilesQuery("Table1", true, false, false);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void BuildGetFilesQuery_RecursiveTrue_ExcludeDataTrue_ExcludeDirectoriesTrue_Test()
        {
            // Arrange
            var repo = new FileTableRepo();
            var expected = "SELECT [stream_id], null as [file_stream],[name],[path_locator],[parent_path_locator],[file_type],[cached_file_size],[creation_time],[last_write_time],[last_access_time],[is_directory],[is_offline],[is_hidden],[is_readonly],[is_archive],[is_system],[is_temporary] FROM Table1 WHERE path_locator.IsDescendantOf(@dirId) = 1 AND is_directory = 0";

            // Act
            var actual = repo.BuildGetFilesQuery("Table1", true, true, true);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
