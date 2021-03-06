﻿using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Tutor
{
    public class CalendarEventsQuery : IQuery<CalendarEventDto>
    {
        public CalendarEventsQuery(long id, DateTime @from, DateTime to)
        {
            From = new DateTime(@from.Year, @from.Month, @from.Day,
                @from.Hour, 0, 0, @from.Kind);
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

            public CalendarEventsQueryHandler(IStatelessSession session, ICalendarService calendarService)
            {
                _calendarService = calendarService;
                _statelessSession = session;
            }

            public async Task<CalendarEventDto> GetAsync(CalendarEventsQuery query, CancellationToken token)
            {

                var calendarsFuture = _statelessSession.Query<TutorCalendar>()
                    .WithOptions(w => w.SetComment(nameof(CalendarEventsQuery)))
                    .Where(w => w.Tutor.Id == query.Id)
                    .Select(s => s.Calendar.GoogleId).ToFuture();

                var availableFuture = _statelessSession.Query<TutorHours>()
                    .Where(w => w.Tutor.Id == query.Id)
                    .ToFuture();

                var calendars = await calendarsFuture.GetEnumerableAsync(token);
                var googleBusySlot = (await _calendarService.ReadCalendarEventsAsync(query.Id, calendars, query.From, query.To, token)).ToList();

                var available = (await availableFuture.GetEnumerableAsync(token)).ToList();

                var result = DateTimeHelpers.EachHour(query.From, query.To).Where(w =>
                {

                    var freeSlot = available.Where(w2 => w.ToUniversalTime().DayOfWeek == w2.AvailabilitySlot.Day);

                    if (googleBusySlot.Any(a => a.From <= w && w < a.To))
                    {
                        return true;
                    }

                    if (available.Count == 0)
                    {
                        return false;
                    }
                    //if (busySlots.Count >0)
                    if (freeSlot.Any(a => a.AvailabilitySlot.From <= w.ToUniversalTime().TimeOfDay && w.ToUniversalTime().TimeOfDay < a.AvailabilitySlot.To))
                    {

                        // Im not busy busy
                        return false;
                    }
                    return true;


                });
                return new CalendarEventDto(result);
            }
        }
    }
}