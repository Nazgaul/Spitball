using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminAssignToQuery : IQuery<IList<string>>
    {

        internal sealed class AdminAssignToQueryHandler : IQueryHandler<AdminAssignToQuery, IList<string>>
        {
            private readonly IStatelessSession _session;

            public AdminAssignToQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }


            public async Task<IList<string>> GetAsync(AdminAssignToQuery query, CancellationToken token)
            {
                return await _session.Query<AdminUser>()
                    .Select(s => s.Email)
                    .ToListAsync(token);


            }
        }
    }
}
