using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Dapper;


namespace Cloudents.Query.Query
{
    public class CourseSearchQuery : IQuery<IEnumerable<CourseDto>>
    {
        public CourseSearchQuery(long userId, string term)
        {
            UserId = userId;
            Term = term;
        }

        public long UserId { get; }
        public string Term { get; }


       
        internal sealed class CoursesByTermQueryHandler : IQueryHandler<CourseSearchQuery, IEnumerable<CourseDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public CoursesByTermQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(CourseSearchQuery query, CancellationToken token)
           {
                var sql = @"select top 50 Name,
	case when uc.CourseId is not null and uc.UserId = @Id then cast(1 as bit) else cast(0 as bit) end as IsRegistered,
	count(distinct uc.UserId) as Users
from sb.Course c
left join sb.UsersCourses uc
	on c.Name = uc.CourseId --and uc.UserId = @Id
where CONTAINS(Name, @Term) and State = 'OK'
group by Name,case when uc.CourseId is not null and uc.UserId = @Id then cast(1 as bit) else cast(0 as bit) end
order by count(distinct uc.UserId) desc";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    return await conn.QueryAsync<CourseDto>(sql, new { Term = query.Term, Id = query.UserId});
                }
                    /*return await _session.Query<Course>()
                    .Where(w => w.Id.Contains(query.Term) && w.State == ItemState.Ok)
                    .OrderByDescending(o => o.Count)
                    .Take(10).Select(s => new CourseDto(s.Id)).ToListAsync(token);*/

            }
        }
    }
    
}