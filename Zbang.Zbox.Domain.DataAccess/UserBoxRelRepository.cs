using System.Collections.Generic;
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

        public IEnumerable<long> GetUserIdsConnectedToBox(long boxId)
        {
            return UnitOfWork.CurrentSession.QueryOver<UserBoxRel>()
                .Where(w => w.Box.Id == boxId).Select(s => s.Id).List<long>();

        }
    }
}
