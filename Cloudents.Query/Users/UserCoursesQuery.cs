using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Users
{
    public class UserCoursesQuery : IQuery<IEnumerable<UserCourseDto>>
    {
        public UserCoursesQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; set; }

        internal sealed class UserCoursesQueryHandler : IQueryHandler<UserCoursesQuery, IEnumerable<UserCourseDto>>
        {

            private readonly IStatelessSession _statelessSession;

            public UserCoursesQueryHandler(QuerySession dapperRepository)
            {
                _statelessSession = dapperRepository.StatelessSession;
            }

            public async Task<IEnumerable<UserCourseDto>> GetAsync(UserCoursesQuery query, CancellationToken token)
            {
                return await _statelessSession.Query<UserCourse2>()
                    .Fetch(f => f.Course)
                    .Fetch(f => f.User)
                    .Where(w => w.User.Id == query.UserId)
                    
                    .Select(s => new UserCourseDto
                    {
                        Name = s.Course.CardDisplay,
                        IsPending = s.Course.State == ItemState.Pending,
                        Students = s.Course.Count,
                        IsTeaching = s.User.Tutor != null
                    })
                    .OrderByDescending(o => o.IsPending).ThenByDescending(o=>o.Students)
                    .ToListAsync(token);

                //token.ThrowIfCancellationRequested();
                ////We use Students, IsPending and IsTeaching in "My Courses" when a user edit his courses list
                //const string sql = @"select CourseId as [Name], 
                //        c.count as Students,
                //        case when c.State = 'Pending' then 1 else null end as IsPending,
                //        uc.CanTeach as IsTeaching
                //        from sb.UsersCourses uc
                //        join sb.Course c
                //        on uc.courseId = c.Name
                //        where UserId = @Id
                //        order by IsPending desc, Students desc";
                //using var conn = _dapperRepository.OpenConnection();
                //return await conn.QueryAsync<UserCourseDto>(sql, new { Id = query.UserId });
            }
        }
    }
}
