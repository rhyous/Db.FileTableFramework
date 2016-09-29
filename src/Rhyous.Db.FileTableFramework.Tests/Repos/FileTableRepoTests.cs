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
    }
}
