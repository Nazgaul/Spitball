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
                                case when uc.CourseId is not null and uc.UserId = @id 
                                    then cast(1 as bit) 
                                    else cast(0 as bit) end as IsRegistered,
	                            Count(distinct u.Id) as Users
                            from sb.Course c
                            left join sb.UsersCourses uc
	                            on c.Name = uc.CourseId
                            left join sb.[User] u
	                            on uc.UserId = U.Id and U.UniversityId2 = @UniversityId
                            where State = 'OK'
                            group by c.Name, case when uc.CourseId is not null and uc.UserId = @id 
                                    then cast(1 as bit) 
                                    else cast(0 as bit) end 
                            order by Count(distinct u.Id) desc;
                            else
                            select top 50 c.Name, 
                                case when uc.CourseId is not null and uc.UserId = @id 
                                    then cast(1 as bit) 
                                    else cast(0 as bit) end as IsRegistered,
	                            Count(distinct u.Id) as Users
                            from sb.Course c
                            left join sb.UsersCourses uc
	                            on c.Name = uc.CourseId
                            left join sb.[User] u
	                            on uc.UserId = U.Id
                            where State = 'OK'
                            group by c.Name, case when uc.CourseId is not null and uc.UserId = @id 
                                    then cast(1 as bit) 
                                    else cast(0 as bit) end 
                            order by Count(distinct u.Id) desc;";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    return await conn.QueryAsync<CourseDto>(sql, new { UniversityId = query.UniversityId, Id = query.UserId });
                }
            }
        }
    }
}
