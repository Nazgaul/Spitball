using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Tutor
{
    public class UpcomingLessonsQuery : IQuery<IEnumerable<UpcomingStudyRoomDto>>
    {
        public UpcomingLessonsQuery(long tutorId)
        {
            TutorId = tutorId;
        }

        public long TutorId { get;  }

        internal sealed class UpcomingLessonsQueryHandler : IQueryHandler<UpcomingLessonsQuery,IEnumerable<UpcomingStudyRoomDto>>
        {
            private readonly IStatelessSession _statelessSession;

            public UpcomingLessonsQueryHandler(IStatelessSession statelessSession)
            {
                _statelessSession = statelessSession;
            }

            public async Task<IEnumerable<UpcomingStudyRoomDto>> GetAsync(UpcomingLessonsQuery query, CancellationToken token)
            {
                return await _statelessSession.Query<BroadCastStudyRoom>()
                    .Where(w => w.Course.Tutor.Id == query.TutorId)
                    .Select(s => new UpcomingStudyRoomDto()
                    {
                        BroadcastTime = s.BroadcastTime,
                        CourseName = s.Course.Name,
                        StudyRoomId = s.Id,
                        CourseId = s.Course.Id,
                        StudentEnroll = s.Users.Count(),
                        StudyRoomName = s.Description
                    }).ToListAsync(token);
            }
        }
    }
}