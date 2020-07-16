using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Tutor
{
    public class CalendarListQuery : IQuery<IEnumerable<CalendarDto>>
    {
        public CalendarListQuery(long userId)
        {
            UserId = userId;
        }
        private long UserId { get; }

        internal sealed class CalendarListQueryHandler : IQueryHandler<CalendarListQuery, IEnumerable<CalendarDto>>
        {
            private readonly IStatelessSession _statelessSession;
            private readonly ICalendarService _calendarService;

            public CalendarListQueryHandler(IStatelessSession session, ICalendarService calendarService)
            {
                _calendarService = calendarService;
                _statelessSession = session;
            }

            public async Task<IEnumerable<CalendarDto>> GetAsync(CalendarListQuery query, CancellationToken token)
            {
                var taskGoogleCalendarResult = _calendarService.GetUserCalendarsAsync(query.UserId, token);
                var taskSharedCalendarResult = _statelessSession.Query<TutorCalendar>()
                    .WithOptions(w => w.SetComment(nameof(CalendarListQuery)))
                     .Where(w => w.Tutor.Id == query.UserId)
                     .Select(s => new { s.Calendar.GoogleId })
                     .ToListAsync(token);

                await Task.WhenAll(taskSharedCalendarResult, taskGoogleCalendarResult);

                var googleCalendarResult = await taskGoogleCalendarResult;
                var sharedCalendarResult = await taskSharedCalendarResult;

                return googleCalendarResult.Select(s =>
                {
                    s.IsShared = sharedCalendarResult.Any(a => a.GoogleId == s.Id);
                    return s;
                });
            }
        }
    }
}
