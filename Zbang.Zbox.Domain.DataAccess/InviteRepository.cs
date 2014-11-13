using System.Linq;
using System.Collections.Generic;
using NHibernate.Criterion;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class InviteRepository : NHibernateRepository<Invite>, IInviteRepository
    {
        public InviteToBox GetUserInvite(UserBoxRel userBoxRel)
        {
            return UnitOfWork.CurrentSession.QueryOver<InviteToBox>()
                .Where(w => w.UserBoxRel == userBoxRel && !w.IsUsed)
                .SingleOrDefault();
        }

        public IEnumerable<InviteToBox> GetUserInvites(long recipientId)
        {
            var userBoxRels =  UnitOfWork.CurrentSession.QueryOver<UserBoxRel>()
                .Where(w => w.User.Id == recipientId).List().ToArray();
            return UnitOfWork.CurrentSession.QueryOver<InviteToBox>().Where(Restrictions.On<InviteToBox>(s=>s.UserBoxRel).IsIn(userBoxRels)).List();
        }
    }
}
