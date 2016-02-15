using System;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface ILibraryRepository :  IRepository<Library>
    {
        Guid GetTopTreeNode(Guid departmentId);

        void UpdateElementToIsDirty(Guid departmentId);
    }
}
