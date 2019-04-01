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
        public CourseSearchByUniversityQuery(Guid? universityId)
        {
            UniversityId = universityId;
        }
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
                            select top 50 c.Name, cast(0 as bit) as IsRegistered,
	                            Count(distinct u.Id) as Users
                            from sb.Course c
                            left join sb.UsersCourses uc
	                            on c.Name = uc.CourseId --and uc.UserId = @Id
                            left join sb.[User] u
	                            on uc.UserId = U.Id and U.UniversityId2 = @UniversityId
                            where State = 'OK'
                            group by c.Name
                            order by Count(distinct u.Id) desc;
                            else
                            select top 50 c.Name, cast(0 as bit) as IsRegistered,
	                            Count(distinct u.Id) as Users
                            from sb.Course c
                            left join sb.UsersCourses uc
	                            on c.Name = uc.CourseId --and uc.UserId = @Id
                            left join sb.[User] u
	                            on uc.UserId = U.Id
                            where State = 'OK'
                            group by c.Name
                            order by Count(distinct u.Id) desc;";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    return await conn.QueryAsync<CourseDto>(sql, new { UniversityId = query.UniversityId });
                }
            }
        }
    }
}
