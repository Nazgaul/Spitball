using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

            //[Cache(TimeConst.Day, "homepage4", false)]
            public async Task<StatsDto> GetAsync(StatsQuery query, CancellationToken token)
            {
                var documents = _session.Query<Document>().GroupBy(g => 1).Select(s => s.Count()).ToFutureValue();

                var tutors = _session.Query<Core.Entities.Tutor>().Where(w => w.State == Core.Enum.ItemState.Ok)
                    .GroupBy(g => 1).Select(s => s.Count())
                    .ToFutureValue();

                var usersUploadedDocs = _session.Query<Document>().Where(w => w.Status.State == Core.Enum.ItemState.Ok).Select(s => s.User.Id)
                     .GroupBy(g => 1).Select(s => s.Distinct().Count())
                    .ToFutureValue();

                var users = _session.Query<User>().Where(w => w.EmailConfirmed).GroupBy(g => 1).Select(s => s.Count()).ToFutureValue();
                var reviews = _session.Query<TutorReview>().GroupBy(g => 1)
                    .Select(s => new { sum = s.Sum(x => x.Rate), count = s.Count() }).ToFutureValue();

                var document = await documents.GetValueAsync(token);
                return new StatsDto
                {
                    Documents = document,
                    Tutors = tutors.Value + usersUploadedDocs.Value,

                    Students = users.Value,
                    Reviews = reviews.Value.sum / reviews.Value.count / 5
                };
            }
        }
    }
}