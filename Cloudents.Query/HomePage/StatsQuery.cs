﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.HomePage
{
    public class StatsQuery : IQuery<StatsDto>
    {
        //[SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Used for cache")]
        //private CultureInfo CultureInfo => CultureInfo.CurrentCulture;

        internal sealed class StatsQueryHandler : IQueryHandler<StatsQuery, StatsDto>
        {
            private readonly IStatelessSession _session;

            public StatsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            [Cache(TimeConst.Day, "homepage4", false)]
            public async Task<StatsDto> GetAsync(StatsQuery query, CancellationToken token)
            {
                var documents = _session.Query<Document>().GroupBy(g => 1).Select(s => s.Count()).ToFutureValue();
                var tutors = _session.Query<Core.Entities.Tutor>().GroupBy(g => 1).Select(s => s.Count())
                    .ToFutureValue();
                var users = _session.Query<User>().Where(w => w.EmailConfirmed).GroupBy(g => 1).Select(s => s.Count()).ToFutureValue();
                var reviews = _session.Query<TutorReview>().GroupBy(g => 1)
                    .Select(s => new { sum = s.Sum(x => x.Rate), count = s.Count() }).ToFutureValue();

                var document = await documents.GetValueAsync(token);
                return new StatsDto
                {
                    Documents = document,
                    Tutors = tutors.Value,

                    Students = users.Value,
                    Reviews = reviews.Value.sum / reviews.Value.count / 5
                };
            }
        }
    }
}