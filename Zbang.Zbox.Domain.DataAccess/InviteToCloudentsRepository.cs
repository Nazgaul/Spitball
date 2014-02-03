using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class InviteToCloudentsRepository : NHibernateRepository<InviteToCloudents>, IInviteToCloudentsRepository
    {
        public InviteToCloudents GetInviteToCloudents(User recepient)
        {
            return UnitOfWork.CurrentSession.QueryOver<InviteToCloudents>()
                .Where(w => w.Recepient == recepient)
                .SingleOrDefault();
        }
    }
}
