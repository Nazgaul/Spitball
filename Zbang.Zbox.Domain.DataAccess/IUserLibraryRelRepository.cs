using System;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IUserLibraryRelRepository : IRepository<UserLibraryRel>
    {
        UserLibraryRel GetUserLibraryRelationship(long userId, Guid departmentId);
    }
}