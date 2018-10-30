using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query 
{
    class UniversityQueryHandler : IQueryHandler<UniversityQuery, IEnumerable<UniversityDto>>
    {

        private readonly IStatelessSession _session;

        public UniversityQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }


        public async Task<IEnumerable<UniversityDto>> GetAsync(UniversityQuery query, CancellationToken token)
        {

            return await _session.Query<User>()
                 .Fetch(f => f.University)
                 .Where(w => w.Id == query.UserId)
                 .Select(s => new UniversityDto(s.University.Id, s.University.Name, s.University.Country)).ToListAsync(token);
              
        }

    }
}
