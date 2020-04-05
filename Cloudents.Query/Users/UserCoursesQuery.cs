using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserCoursesQuery : IQuery<IEnumerable<CourseDto>>
    {
        public UserCoursesQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; set; }

        internal sealed class UserCoursesQueryHandler : IQueryHandler<UserCoursesQuery, IEnumerable<CourseDto>>
        {
            private readonly IDapperRepository _dapperRepository;

            public UserCoursesQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(UserCoursesQuery query, CancellationToken token)
            {
                token.ThrowIfCancellationRequested();
                //We use Students, IsPending and IsTeaching in "My Courses" when a user edit his courses list
                const string sql = @"select CourseId as [Name], 
                        c.count as Students,
                        case when c.State = 'Pending' then 1 else null end as IsPending,
                        uc.CanTeach as IsTeaching
                        from sb.UsersCourses uc
                        join sb.Course c
                        on uc.courseId = c.Name
                        where UserId = @Id
                        order by IsPending desc, Students desc";
                using var conn = _dapperRepository.OpenConnection();
                return await conn.QueryAsync<CourseDto>(sql, new { Id = query.UserId });
            }
        }
    }
}
