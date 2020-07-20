using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Tutor
{
    public class TutorUpcomingBroadcastStudyRoomQuery : IQuery<IEnumerable<FutureBroadcastStudyRoomDto>>
    {
        public TutorUpcomingBroadcastStudyRoomQuery(long tutorId, long userId)
        {
            TutorId = tutorId;
            UserId = userId;
        }

        private long TutorId { get; }
        private long UserId { get; }


        internal sealed class TutorUpcomingBroadcastStudyRoomQueryHandler : IQueryHandler<TutorUpcomingBroadcastStudyRoomQuery, IEnumerable<FutureBroadcastStudyRoomDto>>
        {
            private readonly IStatelessSession _session;
            private readonly ICronService _cronService;

            public TutorUpcomingBroadcastStudyRoomQueryHandler(IStatelessSession session, ICronService cronService)
            {
                _session = session;
                _cronService = cronService;
            }
            public async Task<IEnumerable<FutureBroadcastStudyRoomDto>> GetAsync(TutorUpcomingBroadcastStudyRoomQuery query, CancellationToken token)
            {
                const int studyRoomMaxUsers = 48;
                var result = await _session.Query<BroadCastStudyRoom>()
                    .Where(w => w.Tutor.Id == query.TutorId &&
                                //Add to broadcast time date is not supported
                                w.BroadcastTime > DateTime.UtcNow.AddHours(-1))
                    .OrderBy(o => o.BroadcastTime)
                    .Select(s => new FutureBroadcastStudyRoomDto()
                    {
                        Id = s.Id,
                        Price = s.Price,
                        Name = s.Name,
                        DateTime = s.BroadcastTime,
                        Description = s.Description,
                        IsFull = _session.Query<StudyRoomUser>().Count(w => w.Room.Id == s.Id) >= studyRoomMaxUsers,
                        Enrolled = _session.Query<StudyRoomUser>()
                            .Any(w => w.Room.Id == s.Id && w.User.Id == query.UserId),
                        Schedule = s.Schedule
                    }).ToListAsync(token);

                return result.Select(s =>
                 {
                     if (s.Schedule != null)
                     {
                         s.NextEvents = _cronService.GetNextOccurrences(s.Schedule.CronString,
                             s.Schedule.Start, s.Schedule.End);
                     }

                     return s;
                 });





            }
        }
    }
}