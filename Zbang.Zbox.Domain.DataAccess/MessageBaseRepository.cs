using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Data.NHibernameUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class MessageBaseRepository : NHibernateRepository<MessageBase>, IMessageBaseRepository
    {
        public IEnumerable<MessageBase> GetCurrentInvites(long recepientId)
        {
            return UnitOfWork.CurrentSession.QueryOver<MessageBase>()
                .Where(w => w.Recepient.Id == recepientId).List();
        }
    }
}
