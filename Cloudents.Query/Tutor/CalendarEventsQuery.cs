using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;
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

                var calendarsFuture =  _statelessSession.Query<TutorCalendar>().Where(w => w.Tutor.Id == query.Id)
                    .Select(s => s.GoogleId).ToFuture();

                var availableFuture = _statelessSession.Query<TutorHours>()
                    .Where(w => w.Tutor.Id == query.Id)
                    .ToFuture();

                var calendars = await calendarsFuture.GetEnumerableAsync(token);
                var googleBusySlot = await _calendarService.ReadCalendarEventsAsync(query.Id, calendars, query.From, query.To, token);

                var available = (await availableFuture.GetEnumerableAsync(token)).ToList();

                var result = DateTimeHelpers.EachHour(query.From, query.To).Where(w =>
                {
                    var busySlots = available.Where(w2 => w.DayOfWeek == w2.WeekDay);

                    //if (busySlots.Count >0)
                    if (busySlots.Any(a => a.From < w.TimeOfDay && w.TimeOfDay < a.To))
                    {
                        // Im busy
                        return true;
                    }

                    if (googleBusySlot.Any(a => a.From <= w && w <= a.To))
                    {
                        return true;
                    }
                    return false;


                });
                //List<DateTime> res = new List<DateTime>();
                //var date = query.From;
        
                //while (date < query.To)
                //{
                //    if (date.Date != date.AddHours(-1).Date)
                //    {
                //        available = await _statelessSession.Query<TutorHours>()
                //        .Where(w => w.Tutor.Id == query.Id)
                //        .Where(w => w.WeekDay == date.DayOfWeek)
                //        .ToListAsync();
                //    }
                //    if (available != null)
                //    {
                //        foreach (var item in available)
                //        {
                //            if (date.Hour < item.From.Hours || date.Hour > item.To.Hours
                //                || googleBusySlot.BusySlot.Contains(date))
                //            {
                //                res.Add(date);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        res.Add(date);
                //    }
                //    date = date.AddHours(1);

                //}

                return new CalendarEventDto(result);
            }
        }
    }
}