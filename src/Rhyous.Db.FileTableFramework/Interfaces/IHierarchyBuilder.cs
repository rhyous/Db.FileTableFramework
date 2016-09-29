using System;

namespace Rhyous.Db.FileTableFramework.Interfaces
{
    public interface IHierarchyBuilder
    {
        string NewChildHierarchyId(string pathId);
        string NewChildHierarchyId(string pathId, Guid guid);
    }
}
