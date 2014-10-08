using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class InviteRepository : NHibernateRepository<Invite>, IInviteRepository
    {
        public Invite GetCurrentInvite(User recepient, Box box)
        {
            return UnitOfWork.CurrentSession.QueryOver<Invite>()
                .Where(w => w.Recipient == recepient && w.Box == box && w.IsActive)
                .SingleOrDefault();
        }
    }
}
