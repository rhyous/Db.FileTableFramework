using Microsoft.SqlServer.Types;
using System;

namespace Rhyous.Db.FileTableFramework.Interfaces
{
    public interface IHierarchyBuilder
    {
        SqlHierarchyId NewChildHierarchyId(SqlHierarchyId pathId);
        SqlHierarchyId NewChildHierarchyId(SqlHierarchyId pathId, Guid guid);
    }
}
