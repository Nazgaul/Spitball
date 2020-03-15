using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class ReferUserTransactionRepository : NHibernateRepository<ReferUserTransaction>, IReferUserTransactionRepository
    {
        public ReferUserTransactionRepository(ISession session) : base(session)
        {
        }

        public async Task<int> GetReferUserCountAsync(long userId, CancellationToken token)
        {
            return await Session.Query<ReferUserTransaction>()
                 .CountAsync(w => w.User.Id == userId, token);
        }
    }
}
