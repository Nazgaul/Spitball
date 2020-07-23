using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;


namespace Cloudents.Query.Courses
{
    public class CourseSearchQuery : IQuery<IEnumerable<CourseDto>>
    {
        public CourseSearchQuery(long userId,  string term)
        {
            UserId = userId;
            Term = term;
        }

        private long UserId { get; }
        private string Term { get; }



        internal sealed class CoursesByTermQueryHandler : IQueryHandler<CourseSearchQuery, IEnumerable<CourseDto>>
        {
            private readonly IStatelessSession _statelessSession;

            public CoursesByTermQueryHandler(IStatelessSession statelessSession)
            {
                _statelessSession = statelessSession;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(CourseSearchQuery query, CancellationToken token)
            {
                var dbQuery = _statelessSession.Query<Course>()
                    .Where(w => w.Tutor.Id == query.UserId);

                if (!string.IsNullOrEmpty(query.Term))
                {
                    dbQuery = dbQuery.Where(w => w.Name.Like(query.Term));
                }

                return await dbQuery.Select(s => new CourseDto()
                {
                    Name = s.Name

                }).ToListAsync(token);

            }
        }
    }

}