using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Query
{
    public class CourseSearchQuery : IQuery<IEnumerable<CourseDto>>
    {
        public CourseSearchQuery(string term)
        {
            Term = term;
        }

        public string Term { get; }


       
        internal sealed class CoursesByTermQueryHandler : IQueryHandler<CourseSearchQuery, IEnumerable<CourseDto>>
        {
            private readonly IStatelessSession _session;

            public CoursesByTermQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(CourseSearchQuery query, CancellationToken token)
            {
                return await _session.Query<Course>()
                    .Where(w => w.Id.Contains(query.Term) && w.State == ItemState.Ok)
                    .OrderByDescending(o => o.Count)
                    .Take(10).Select(s => new CourseDto(s.Id)).ToListAsync(token);

            }
        }
    }
    
}