using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Tutor
{
    public class TutorActionsQuery : IQuery<TutorActionsDto>
    {
        public TutorActionsQuery(long userId)
        { 
            UserId = userId;
        }
        public long UserId { get; }

        internal sealed class TutorActionsQueryHandler : IQueryHandler<TutorActionsQuery, TutorActionsDto>
        {
            private readonly IStatelessSession _session;
            public TutorActionsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public async Task<TutorActionsDto> GetAsync(TutorActionsQuery query, CancellationToken token)
            {
                var calendarFuture = _session.Query<GoogleTokens>()
                    .Where(w => w.Id == query.UserId.ToString())
                    .Take(1)
                    .ToFutureValue();

                var hoursFuture = _session.Query<TutorHours>()
                    .Where(w => w.Tutor.Id == query.UserId)
                    .Take(1)
                    .ToFutureValue();

                var calendarShared = (await calendarFuture.GetValueAsync(token)) != null ? true : false;
                var haveHours = (await hoursFuture.GetValueAsync(token)) != null ? true : false;

                var res = new TutorActionsDto()
                {
                    CalendarShared = calendarShared,
                    HaveHours = haveHours
                };

                return res;
            }
        }
    }
}
