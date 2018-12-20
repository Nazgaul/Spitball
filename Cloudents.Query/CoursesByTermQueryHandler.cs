using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query
{
    public class CoursesByTermQueryHandler : IQueryHandler<CourseSearchQuery, IEnumerable<CourseDto>>
    {
        private readonly IStatelessSession _session;

        public CoursesByTermQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<CourseDto>> GetAsync(CourseSearchQuery query, CancellationToken token)
        {
            return await _session.Query<Course>()
                //.Where(w => w.Name.IsLike(query.Term,MatchMode.End))
                .Where(w => w.Name.StartsWith(query.Term))
                .OrderByDescending(o => o.Count)
                .Take(10).Select(s => new CourseDto(s.Name)).ToListAsync(token);
        }
    }
}