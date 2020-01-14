using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Query
{
    public class UserCalendarByIdQuery : IQuery<DashboardCalendarDto>
    {
        public UserCalendarByIdQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; }

        internal sealed class UserCalendarByIdQueryHandler : IQueryHandler<UserCalendarByIdQuery, DashboardCalendarDto>
        {
            private readonly IStatelessSession _session;

            public UserCalendarByIdQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<DashboardCalendarDto> GetAsync(UserCalendarByIdQuery query, CancellationToken token)
            {
                var googleCalendarFuture = _session.Query<GoogleTokens>().Where(w => w.Id == query.UserId.ToString()).ToFutureValue();

                var tutorHoursFuture = _session.Query<TutorHours>().Where(w => w.Tutor.Id == query.UserId)
                    .Select(s => new TutorAvailabilitySlot(s.WeekDay, s.From, s.To)).ToFuture();

                var calendar = await googleCalendarFuture.GetValueAsync(token);

                var tutorHours = tutorHoursFuture.GetEnumerable();

                return new DashboardCalendarDto()
                {
                    CalendarShared = calendar != null,
                    TutorDailyHours = tutorHours
                };
            }
        }
    }


}