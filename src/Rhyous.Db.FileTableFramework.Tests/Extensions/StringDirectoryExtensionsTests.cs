using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhyous.Db.FileTableFramework.Extensions;

namespace Rhyous.Db.FileTableFramework.Tests.Extensions
{
    [TestClass]
    public class StringDirectoryExtensionsTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", @"Data\GetRelativePathTestData.csv", "GetRelativePathTestData#csv", DataAccessMethod.Sequential)]
        public void RootIsNotIncluded()
        {
            // Arrange
            var root = TestContext.DataRow[0].ToString();
            var path = TestContext.DataRow[1].ToString();
            var expected = TestContext.DataRow[2].ToString();

            // Act
            var actual = path.GetRelativePath(root);

            // Assert
            Assert.AreEqual(expected, actual);

        }
    }
}
