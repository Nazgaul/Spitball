using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
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
}
