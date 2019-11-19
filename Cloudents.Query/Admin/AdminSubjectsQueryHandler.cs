using Cloudents.Core.Entities;
using Cloudents.Query.Query.Admin;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class AdminSubjectsQueryHandler : IQueryHandler<AdminSubjectsQuery, IList<string>>
    {
        private readonly IStatelessSession _session;

        public AdminSubjectsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IList<string>> GetAsync(AdminSubjectsQuery query,
            CancellationToken token)
        {
            var res = await _session.Query<CourseSubject>()
                .Select(s => s.Name).ToListAsync(cancellationToken: token);
            return res;
        }
    }
}
