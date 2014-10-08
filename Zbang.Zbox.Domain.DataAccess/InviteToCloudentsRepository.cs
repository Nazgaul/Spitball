using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class InviteToCloudentsRepository : NHibernateRepository<InviteToCloudents>, IInviteToCloudentsRepository
    {
        public InviteToCloudents GetInviteToCloudents(User recipient)
        {
            return UnitOfWork.CurrentSession.QueryOver<InviteToCloudents>()
                .Where(w => w.Recipient == recipient)
                .Take(1).SingleOrDefault();
        }
    }
}
