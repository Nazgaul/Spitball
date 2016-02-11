using System;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IUserBoxRelRepository : IRepository<UserBoxRel>
    {
        UserBoxRel GetUserBoxRelationship(long userId, long boxId);
    }

    public interface IUserLibraryRelRepository : IRepository<UserLibraryRel>
    {
        UserLibraryRel GetUserLibraryRelationship(long userId, Guid departmentId);
    }
}
