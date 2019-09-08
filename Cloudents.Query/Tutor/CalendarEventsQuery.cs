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
    public class CalendarEventsQuery : IQuery<CalendarEventDto>
    {
        public CalendarEventsQuery(long id, DateTime @from, DateTime to)
        {
            From = @from;
            To = to;
            Id = id;
        }

        private DateTime From { get; }
        private DateTime To { get; }

        private long Id { get; }


        internal sealed class CalendarEventsQueryHandler : IQueryHandler<CalendarEventsQuery, CalendarEventDto>
        {
            private readonly IStatelessSession _statelessSession;
            private readonly ICalendarService _calendarService;

            public CalendarEventsQueryHandler(QuerySession session, ICalendarService calendarService)
            {
                _calendarService = calendarService;
                _statelessSession = session.StatelessSession;
            }

            public async Task<CalendarEventDto> GetAsync(CalendarEventsQuery query, CancellationToken token)
            {

                var calendars = await _statelessSession.Query<TutorCalendar>().Where(w => w.Tutor.Id == query.Id)
                    .Select(s => s.GoogleId).ToListAsync(token);
                return await _calendarService.ReadCalendarEventsAsync(query.Id, calendars, query.From, query.To, token);
            }
        }
    }
}