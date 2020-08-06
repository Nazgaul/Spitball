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
                var documentFuture = await _session.Query<Course>()
                    .WithOptions(w => w.SetComment(nameof(UserCoursesByIdQuery)))
                    .Where(w => w.Tutor.Id == query.Id && w.State != ItemState.Deleted)
                    .OrderByDescending(o=>o.Position)
                    .Select(s => new UserCoursesDto()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Price = s.Price,
                        Users = s.CourseEnrollments.Count(),
                        Documents = s.Documents.Count(c=>c.Status.State == ItemState.Ok),
                        Lessons = s.StudyRooms.Count(),
                        IsPublish = s.State == ItemState.Ok,
                        StartOn = s.StartTime,
                        Version = s.Version
                    }).ToListAsync(token);

             

                return documentFuture;
            }
        }
    }
}
