using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class SubjectsQuery : IQuery<IList<string>>
    {
        internal sealed class SubjectsQueryHandler : IQueryHandler<SubjectsQuery, IList<string>>
        {
            private readonly IStatelessSession _session;

            public SubjectsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IList<string>> GetAsync(SubjectsQuery query,
                CancellationToken token)
            {
                var res = await _session.Query<CourseSubject>()
                    .Select(s => s.Name).ToListAsync(cancellationToken: token);
                return res;
            }
        }
    }
}
