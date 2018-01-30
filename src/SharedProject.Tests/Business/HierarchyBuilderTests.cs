using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhyous.Db.FileTableFramework.Business;
using Microsoft.SqlServer.Types;

namespace Rhyous.Db.FileTableFramework.Tests.Business
{
    [TestClass]
    public class HierarchyBuilderTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
        }
        #region

        //Guid                                  Binary(16)                         SUBSTRING      BIGINT         VARCHAR(20)
        // 588FE337-604A-4A90-B20D-60138E43318D 0x37E38F584A60904AB20D60138E43318D 0x37E38F584A60 61450502031968 61450502031968
        // part1            part2           part3
        // 61450502031968   158650489200659 2386768269
        [TestMethod]
        public void GetLongTest1Part1()
        {
            var guid = new Guid("588FE337-604A-4A90-B20D-60138E43318D");
            var byteArray = guid.ToByteArray();
            var actual = HierarchyBuilder.GetLong(0, 6, byteArray);
            Assert.AreEqual(61450502031968, actual);
        }

        [TestMethod]
        public void GetLongTest1Part2()
        {
            var guid = new Guid("588FE337-604A-4A90-B20D-60138E43318D");
            var byteArray = guid.ToByteArray();
            var actual = HierarchyBuilder.GetLong(6, 6, byteArray);
            Assert.AreEqual(158650489200659, actual);
        }

        [TestMethod]
        public void GetLongTest1Part3()
        {
            var guid = new Guid("588FE337-604A-4A90-B20D-60138E43318D");
            var byteArray = guid.ToByteArray();
            var actual = HierarchyBuilder.GetLong(12, 4, byteArray);
            Assert.AreEqual(2386768269, actual);
        }

        [TestMethod]
        public void NewChildHierarchyIdTest()
        {
            var parentPathId = SqlHierarchyId.Parse("/9958588825279.274102742982672.185216777/");
            var expectedCombinedPathId = SqlHierarchyId.Parse(parentPathId.ToString() + "61450502031968.158650489200659.2386768269/");
            var guid = new Guid("588FE337-604A-4A90-B20D-60138E43318D");
            var actual = new HierarchyBuilder().NewChildHierarchyId(parentPathId, guid);
            Assert.AreEqual(expectedCombinedPathId, actual);
        }


        [TestMethod]
        public void NewChildHierarchyIdNullParentPathIdTest()
        {
            var childPathId = SqlHierarchyId.Parse("/61450502031968.158650489200659.2386768269/");
            var guid = new Guid("588FE337-604A-4A90-B20D-60138E43318D");
            var actual = new HierarchyBuilder().NewChildHierarchyId(SqlHierarchyId.Null, guid);
            Assert.AreEqual(childPathId, actual);
        }
        #endregion
    }
}
