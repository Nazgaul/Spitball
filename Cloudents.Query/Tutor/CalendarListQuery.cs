using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public long UserId { get; set; }

        internal sealed class CalendarListQueryHandler : IQueryHandler<CalendarListQuery, IEnumerable<CalendarDto>>
        {
            private readonly IStatelessSession _statelessSession;
            private readonly ICalendarService _calendarService;

            public CalendarListQueryHandler(QuerySession session, ICalendarService calendarService)
            {
                _calendarService = calendarService;
                _statelessSession = session.StatelessSession;
            }

            public async Task<IEnumerable<CalendarDto>> GetAsync(CalendarListQuery query, CancellationToken token)
            {
                List<CalendarDto> res = (await _calendarService.GetUserCalendarsAsync(query.UserId, token)).ToList();
                List<CalendarDto> shared = await _statelessSession.Query<TutorCalendar>()
                    .Where(w => w.Tutor.Id == query.UserId)
                    .Select(s => new CalendarDto(s.GoogleId, s.Name, true))
                    .ToListAsync(token);

                //var r =  res.Join(shared, res => res.Id, shared => shared.Id,
                //        (res, shared) => new CalendarDto(res.Id, res.Name, shared.Id != null ? true : false)
                //        );

                foreach (var item in res)
                {
                    item.IsShared = shared.Where(w => w.Id == item.Id).FirstOrDefault() == null ? false : true;
                }

                return res;
            }
        }
    }
}
