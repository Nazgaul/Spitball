using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
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


        internal sealed class TutorUpcomingBroadcastStudyRoomQueryHandler: IQueryHandler<TutorUpcomingBroadcastStudyRoomQuery, IEnumerable<FutureBroadcastStudyRoomDto>>
        {
            private readonly IStatelessSession _session;

            public TutorUpcomingBroadcastStudyRoomQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public async Task<IEnumerable<FutureBroadcastStudyRoomDto>> GetAsync(TutorUpcomingBroadcastStudyRoomQuery query, CancellationToken token)
            {
                return await _session.Query<StudyRoom>()
                    .Where(w => w.Tutor.Id == query.TutorId && w.StudyRoomType == StudyRoomType.Broadcast &&
                                w.BroadcastTime > DateTime.UtcNow)
                    .OrderByDescending(o=>o.Id)
                    .Select(s => new FutureBroadcastStudyRoomDto()
                    {
                        Id = s.Id,
                        Price = s.Price.GetValueOrDefault(),
                        Name = s.Name,
                        DateTime = s.BroadcastTime!.Value,
                        Enrolled = _session.Query<StudyRoomUser>().Any(w => w.Room.Id == s.Id && w.User.Id == query.UserId)
                    }).ToListAsync(token);
            }
        }
    }
}