using Cloudents.Core.DTOs;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class UserCoursesQuery : IQuery<IEnumerable<CourseDto>>
    {
        public UserCoursesQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }

        internal sealed class UserCoursesQueryHandler : IQueryHandler<UserCoursesQuery, IEnumerable<CourseDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public UserCoursesQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(UserCoursesQuery query, CancellationToken token)
            {
                var sql = @"select CourseId as [Name], 
                            cast(null as bit) as IsFollowing,
	                        (select count(1) from sb.UsersCourses uc2 where uc2.CourseId = uc.CourseId) as Students,
	                        c.State as [State]
                        from sb.UsersCourses uc
                        join sb.Course c
	                        on uc.courseId = c.Name
                        where UserId = @Id
                        order by 4 desc, 3 desc";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    return await conn.QueryAsync<CourseDto>(sql, new { Id = query.UserId });
                }
            }
        }
    }
}
