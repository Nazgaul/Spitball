using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.StudyRooms;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.StudyRooms
{
    public class LiveClassesReminderQuery : IQuery<(IEnumerable<LiveClassesReminderDto> daysBefore, IEnumerable<LiveClassesReminderDto> tenMinuted)>
    {
        internal sealed class LiveClassesQueryReminderQueryHandler : IQueryHandler<LiveClassesReminderQuery, (IEnumerable<LiveClassesReminderDto> daysBefore, IEnumerable<LiveClassesReminderDto> tenMinuted)>
        {
            private readonly IStatelessSession _session;

            public LiveClassesQueryReminderQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<(IEnumerable<LiveClassesReminderDto> daysBefore, IEnumerable<LiveClassesReminderDto> tenMinuted)> GetAsync(LiveClassesReminderQuery query, CancellationToken token)
            {
                var now = DateTime.UtcNow;

                var tenMinutesFuture = QueryBuilder(now.AddMinutes(10));
                var dayBeforeFuture = QueryBuilder(now.AddDays(1));

                var tenMinutesResult = await tenMinutesFuture.GetEnumerableAsync(token);
                var dayBeforeResult = dayBeforeFuture.GetEnumerable();

                return (dayBeforeResult, tenMinutesResult);

            }

            private IFutureEnumerable<LiveClassesReminderDto> QueryBuilder(DateTime now)
            {
                var roundTo = TimeSpan.FromMinutes(15);
                return _session.Query<BroadCastStudyRoom>()
                     .Where(w => w.BroadcastTime >= now.RoundDown(roundTo) && w.BroadcastTime <= now.RoundUp(roundTo))
                     .SelectMany(s => s.Users, (s, u) => new { s, u })
                     .Select(s => new LiveClassesReminderDto()
                     {
                         StudyRoomId = s.s.Id,
                         BroadCastTime = s.s.BroadcastTime,
                         StudentEmail = s.u.User.Email,
                         StudentFirstName = s.u.User.FirstName,
                         StudyRoomDescription = s.s.Description,
                         TeacherName = s.s.Tutor.User.Name,
                         StudyRoomTitle = s.s.Name,
                         UserId = s.u.User.Id
                             
                     }).ToFuture();

            }
        }
    }
}