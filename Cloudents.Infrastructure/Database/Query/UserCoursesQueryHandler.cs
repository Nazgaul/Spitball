using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Database.Query
{
    public class UserCoursesQueryHandler : IQueryHandler<CoursesQuery, IEnumerable<CourseDto>>
    {
        private readonly IStatelessSession _session;

        public UserCoursesQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }

        public async Task<IEnumerable<CourseDto>> GetAsync(CoursesQuery query, CancellationToken token)
        {
            //V7 - need to check
           return await _session.Query<User>()
                .Fetch(f => f.Courses)
               
                .Where(w=>w.Id == query.UserId)
               .SelectMany(s => s.Courses)
                .Select(s=> new CourseDto
                {
                    Name = s.Name
                }).ToListAsync(token);
        }
    }
}