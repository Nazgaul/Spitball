using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class LeadRepository : NHibernateRepository<Lead>, ILeadRepository
    {
        public LeadRepository(ISession session) : base(session)
        {
        }

        public async Task<IEnumerable<Lead>> GetLeadsByUserIdAsync(long UserId, CancellationToken token)
        {
            return await Session.QueryOver<Lead>()
                .Where(w => w.User.Id == UserId).ListAsync(token);
        }
    }
}
