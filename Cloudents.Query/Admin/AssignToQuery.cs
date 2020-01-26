using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AssignToQuery : IQuery<IList<string>>
    {

        internal sealed class AssignToQueryHandler : IQueryHandler<AssignToQuery, IList<string>>
        {
            private readonly IStatelessSession _session;

            public AssignToQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }


            public async Task<IList<string>> GetAsync(AssignToQuery query, CancellationToken token)
            {
                return await _session.Query<AdminUser>()
                    .Select(s => s.Email)
                    .ToListAsync(token);


            }
        }
    }
}
