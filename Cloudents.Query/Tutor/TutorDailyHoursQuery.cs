using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
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
    public class TutorDailyHoursQuery : IQuery<IEnumerable<TutorDailyHoursDto>>
    {
        public TutorDailyHoursQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; set; }

        internal sealed class TutorDailyHoursQueryHandler : IQueryHandler<TutorDailyHoursQuery, IEnumerable<TutorDailyHoursDto>>
        {
            private readonly IStatelessSession _session;
            public TutorDailyHoursQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<TutorDailyHoursDto>> GetAsync(TutorDailyHoursQuery query, CancellationToken token)
            {
                return await _session.Query<TutorHours>()
                    .Fetch(f => f.Tutor)
                    .Where(w => w.Tutor.Id == query.UserId)
                    .Select(s => new TutorDailyHoursDto()
                    {
                        WeekDay = s.WeekDay,
                        From = s.From,
                        To = s.To
                    }).ToListAsync(token);

            }
        }
    }
}
