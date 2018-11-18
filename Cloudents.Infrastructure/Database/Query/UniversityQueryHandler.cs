using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query
{
    internal class UniversityQueryHandler : IQueryHandler<UniversityQuery, UniversityDto>
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
                 .Select(s => new UniversityDto(s.Id, s.Name, s.Country))
                .SingleOrDefaultAsync(token);

        }

    }
}
