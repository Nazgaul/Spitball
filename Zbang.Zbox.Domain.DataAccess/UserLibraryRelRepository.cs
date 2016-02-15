using System;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
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