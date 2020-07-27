using Cloudents.Core.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;


namespace Cloudents.Query.Courses
{
    public class CourseSearchQuery : IQuery<IEnumerable<CourseNameDto>>
    {
        public CourseSearchQuery(long userId, string? term)
        {
            UserId = userId;
            Term = term;
        }

        private long UserId { get; }
        private string? Term { get; }



        internal sealed class CoursesByTermQueryHandler : IQueryHandler<CourseSearchQuery, IEnumerable<CourseNameDto>>
        {
            private readonly IStatelessSession _statelessSession;

            public CoursesByTermQueryHandler(IStatelessSession statelessSession)
            {
                _statelessSession = statelessSession;
            }

            public async Task<IEnumerable<CourseNameDto>> GetAsync(CourseSearchQuery query, CancellationToken token)
            {
                var dbQuery = _statelessSession.Query<Course>();

                if (query.UserId > 0)
                {
                    dbQuery = dbQuery.Where(w => w.Tutor.Id == query.UserId);
                }


                if (!string.IsNullOrEmpty(query.Term))
                {
                    dbQuery = dbQuery.Where(w => w.Name.StartsWith(query.Term));
                }

                return await dbQuery.Select(s => new CourseNameDto()
                {
                    Name = s.Name

                }).ToListAsync(token);

            }
        }
    }

}