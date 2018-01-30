using Microsoft.SqlServer.Types;
using Rhyous.Db.FileTableFramework.Interfaces;
using System;

namespace Rhyous.Db.FileTableFramework.Business
{
    public class HierarchyBuilder : IHierarchyBuilder
    {
        /// <summary>
        /// Gets new HierachyId as string
        /// </summary>
        /// <param name="pathId">The path id.</param>
        /// <returns></returns>
        public SqlHierarchyId NewChildHierarchyId(SqlHierarchyId pathId)
        {
            return NewChildHierarchyId(pathId, Guid.NewGuid());
        }

        /// <summary>
        /// Gets new HierachyId as string
        /// </summary>
        /// <param name="pathId">The path id.</param>
        /// <param name="guid">The Guid to append to the path id.</param>
        /// <returns></returns>
        public SqlHierarchyId NewChildHierarchyId(SqlHierarchyId pathId, Guid guid)
        {
            var template = "{0}{1}.{2}.{3}/";
            var bytes = guid.ToByteArray();
            if (pathId.IsNull)
                pathId = SqlHierarchyId.Parse("/"); //Root
            
            var hierarchyId = string.Format(template, pathId, GetLong(0, 6, bytes), GetLong(6, 6, bytes), GetLong(12, 4, bytes));
        
            return SqlHierarchyId.Parse(hierarchyId);
        }

        internal static long GetLong(int a, int b, byte[] bytes)
        {
            var subBytes = new byte[8];
            Array.Copy(bytes, a, subBytes, 8 - b, b);
            Array.Reverse(subBytes);
            return BitConverter.ToInt64(subBytes, 0);
        }
    }
}
