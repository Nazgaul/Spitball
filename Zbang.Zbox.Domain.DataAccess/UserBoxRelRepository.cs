using System;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class UserBoxRelRepository : NHibernateRepository<UserBoxRel>, IUserBoxRelRepository
    {
        public UserBoxRel GetUserBoxRelationship(long userId, long boxId)
        {
            return UnitOfWork.CurrentSession.QueryOver<UserBoxRel>().
             Where(w => w.User.Id == userId)
             .Where(w => w.Box.Id == boxId).SingleOrDefault();
        }
    }


    public class UserLibraryRelRepository : NHibernateRepository<UserLibraryRel>, IUserLibraryRelRepository 
    {
        public UserLibraryRel GetUserLibraryRelationship(long userId, Guid departmentId)
        {
            return UnitOfWork.CurrentSession.QueryOver<UserLibraryRel>().
             Where(w => w.User.Id == userId)
             .Where(w => w.Library.Id == departmentId).SingleOrDefault();
        }
    }
}
