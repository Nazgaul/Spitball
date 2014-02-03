using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class InviteRepository : NHibernateRepository<Invite>, IInviteRepository
    {
        public Invite GetCurrentInvite(User recepient, Box box)
        {
            return UnitOfWork.CurrentSession.QueryOver<Invite>()
                .Where(w => w.Recepient == recepient && w.Box == box && w.IsActive)
                .SingleOrDefault();
        }
    }
}
