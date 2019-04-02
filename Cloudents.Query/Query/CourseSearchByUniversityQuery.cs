using Cloudents.Core.DTOs;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class CourseSearchByUniversityQuery : IQuery<IEnumerable<CourseDto>>
    {
        public CourseSearchByUniversityQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }

        internal sealed class CoursesByUniversityQueryHandler : IQueryHandler<CourseSearchByUniversityQuery,
                                                                IEnumerable<CourseDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public CoursesByUniversityQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(CourseSearchByUniversityQuery query, 
                    CancellationToken token)
            {
                var sql = @"select top 50 c.Name, 
                                case when uc.CourseId is not null
                                    then 1 else null end as IsFollowing,
	                            c.count as Students
                            from sb.Course c
                            left join sb.UsersCourses uc
	                            on c.Name = uc.CourseId and uc.UserId = @Id
                            left join sb.[User] u
	                            on uc.UserId = U.Id and U.UniversityId2 = (select UniversityId2 from sb.[User] where id = @Id)
                            where State = 'OK'
                            group by c.Name, 
                                case when uc.CourseId is not null
                                    then 1 else null end,
	                            c.count
                            order by count(distinct u.Id) desc, c.count desc, case when uc.CourseId is not null
                                    then 1 else null end desc;";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    return await conn.QueryAsync<CourseDto>(sql, new { Id = query.UserId });
                }
            }
        }
    }
}
