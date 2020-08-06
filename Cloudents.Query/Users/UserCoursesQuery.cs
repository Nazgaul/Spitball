using Cloudents.Core.DTOs;
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
    public class UserCoursesQuery : IQuery<IEnumerable<CourseDto>>
    {
        public UserCoursesQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; set; }

        internal sealed class UserCoursesQueryHandler : IQueryHandler<UserCoursesQuery, IEnumerable<CourseDto>>
        {
            private readonly IStatelessSession _statelessSession;

            public UserCoursesQueryHandler(IStatelessSession statelessSession)
            {
                _statelessSession = statelessSession;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(UserCoursesQuery query, CancellationToken token)
            {
                var result =  await _statelessSession.Query<Course>()
                    .Where(w => w.Tutor.Id == query.UserId && w.State == ItemState.Ok)
                    .Select(s => new CourseDto
                    {
                        Name = s.Name,
                        Price = s.Price,
                        SubscriptionPrice = s.SubscriptionPrice,
                        Description = s.Description,
                        Id = s.Id,
                        StudyRoomCount = s.StudyRooms.Count(),
                        StartTime = s.StartTime,
                        Version = s.Version
                    }).ToListAsync(token);

                return result;


            }
        }
    }
}
