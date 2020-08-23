using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserCoursesByIdQuery : IQuery<IEnumerable<UserCoursesDto>>
    {
        public UserCoursesByIdQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }

        internal sealed class UserContentByIdQueryHandler : IQueryHandler<UserCoursesByIdQuery, IEnumerable<UserCoursesDto>>
        {
            private readonly IStatelessSession _session;

            public UserContentByIdQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<UserCoursesDto>> GetAsync(UserCoursesByIdQuery query, CancellationToken token)
            {
                var courseFuture = _session.Query<Course>()
                    .WithOptions(w => w.SetComment(nameof(UserCoursesByIdQuery)))
                    .Where(w => w.Tutor.Id == query.Id && w.State != ItemState.Deleted)
                    .OrderBy(o=>o.Position)
                    .Select(s => new UserCoursesDto()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Price = s.Price,
                        //Users =  s.CourseEnrollments.Select(s2 => s2.User.FirstName).ToList(),
                        Documents = s.Documents.Count(c=>c.Status.State == ItemState.Ok),
                        Lessons = s.StudyRooms.Count(),
                        IsPublish = s.State == ItemState.Ok,
                        StartOn = s.StartTime,
                        Version = s.Version
                    }).ToFuture();


                var usersFuture = _session.Query<CourseEnrollment>()
                    .Where(w => w.Course.Tutor.Id == query.Id)
                    .Select(s => new
                    {
                        s.Course.Id,
                        s.User.FirstName
                    }).ToFuture();

                var courses = await courseFuture.GetEnumerableAsync(token);
                var users = usersFuture.GetEnumerable().GroupBy(g => g.Id)
                    .ToDictionary(z => z.Key, v => v.Select(s => s.FirstName));



                return courses.Select(s =>
                {
                    if (users.TryGetValue(s.Id,out var usersFirstNames))
                    {
                        s.Users = usersFirstNames;
                    }

                    return s;
                });
            }
        }
    }
}
