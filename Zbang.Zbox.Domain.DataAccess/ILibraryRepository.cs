using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface ILibraryRepository :  IRepository<Library>
    {
        Guid GetTopTreeNode(Guid departmentId);
    }
}
