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
        public CourseSearchByUniversityQuery(long userId, Guid? universityId)
        {
            UserId = userId;
            UniversityId = universityId;
        }
        public long UserId { get; set; }
        public Guid? UniversityId { get; set; }

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
                var sql = @"if @UniversityId is not null
                            select top 50 c.Name, 
                                case when uc2.CourseId is not null
                                    then cast(1 as bit) 
                                    else cast(0 as bit) end as IsFollowing,
	                            Count(distinct u.Id) as Students
                            from sb.Course c
                            left join sb.UsersCourses uc
	                            on c.Name = uc.CourseId
                            left join sb.[User] u
	                            on uc.UserId = U.Id and U.UniversityId2 = @UniversityId
                            left join sb.UsersCourses uc2
	                            on c.Name = uc2.CourseId and uc2.UserId = @Id
                            where State = 'OK'
                            group by c.Name,uc2.CourseId
                            order by Count(distinct u.Id) desc, case when uc2.CourseId is not null
                                    then cast(1 as bit) 
                                    else cast(0 as bit) end desc;
                            else
                            select top 50 c.Name, 
                                case when uc2.CourseId is not null
                                        then cast(1 as bit) 
                                        else cast(0 as bit) end as IsFollowing,
                                Count(distinct u.Id) as Students
                                from sb.Course c
                                left join sb.UsersCourses uc
                                on c.Name = uc.CourseId
                                left join sb.[User] u
                                on uc.UserId = U.Id
                                left join sb.UsersCourses uc2
	                                on c.Name = uc2.CourseId and uc2.UserId = @Id
                                where State = 'OK'
                                group by c.Name, uc2.CourseId 
                                order by Count(distinct u.Id) desc, case when uc2.CourseId is not null
                                        then cast(1 as bit) 
                                        else cast(0 as bit) end desc;";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    return await conn.QueryAsync<CourseDto>(sql, new { UniversityId = query.UniversityId, Id = query.UserId });
                }
            }
        }
    }
}
