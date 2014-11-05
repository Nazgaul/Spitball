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
    }
}
