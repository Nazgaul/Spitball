using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query
{
    public class UniversityQueryHandler : IQueryHandler<UniversityQuery, UniversityDto>
    {

        private readonly IStatelessSession _session;

        public UniversityQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }


        public Task<UniversityDto> GetAsync(UniversityQuery query, CancellationToken token)
        {

            return _session.Query<University>()
                 .Where(w => w.Id == query.UniversityId)
                 .Select(s => new UniversityDto(s.Id, s.Name))
                .SingleOrDefaultAsync(token);

        }

    }
}
