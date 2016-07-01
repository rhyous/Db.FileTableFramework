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

        #region

        //Guid                                  Binary(16)                         SUBSTRING      BIGINT         VARCHAR(20)
        // 588FE337-604A-4A90-B20D-60138E43318D 0x37E38F584A60904AB20D60138E43318D 0x37E38F584A60 61450502031968 61450502031968
        // part1            part2           part3
        // 61450502031968	158650489200659	2386768269
        [TestMethod]
        public void GetLongTest1Part1()
        {
            var guid = new Guid("588FE337-604A-4A90-B20D-60138E43318D");
            var byteArray = guid.ToByteArray();
            var actual = FileTableRepo.GetLong(0, 6, byteArray);
            Assert.AreEqual(61450502031968, actual);
        }

        [TestMethod]
        public void GetLongTest1Part2()
        {
            var guid = new Guid("588FE337-604A-4A90-B20D-60138E43318D");
            var byteArray = guid.ToByteArray();
            var actual = FileTableRepo.GetLong(6, 6, byteArray);
            Assert.AreEqual(158650489200659, actual);
        }

        [TestMethod]
        public void GetLongTest1Part3()
        {
            var guid = new Guid("588FE337-604A-4A90-B20D-60138E43318D");
            var byteArray = guid.ToByteArray();
            var actual = FileTableRepo.GetLong(12, 4, byteArray);
            Assert.AreEqual(2386768269, actual);
        }

        [TestMethod]
        public void NewChildHierarchyIdTest()
        {
            var parentPathId = "/9958588825279.274102742982672.185216777/";
            var childPathId = "61450502031968.158650489200659.2386768269/";
            var expectedCombinedPathId = parentPathId + childPathId;
            var guid = new Guid("588FE337-604A-4A90-B20D-60138E43318D");
            var actual = new FileTableRepo().NewChildHierarchyId(parentPathId, guid);
            Assert.AreEqual(expectedCombinedPathId, actual);
        }

        #endregion

        [TestMethod]
        public void NewChildHierarchyIdNullParentPathIdTest()
        {
            var childPathId = "/61450502031968.158650489200659.2386768269/";
            var guid = new Guid("588FE337-604A-4A90-B20D-60138E43318D");
            var actual = new FileTableRepo().NewChildHierarchyId(null, guid);
            Assert.AreEqual(childPathId, actual);
        }

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
