using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query
{
    class HomePageQueryHandler : IQueryHandler<HomePageQuery, StatsDto>
    {
        private readonly IStatelessSession _session;

        public HomePageQueryHandler(IStatelessSession session)
        {
            _session = session;
        }
        public async Task<StatsDto> GetAsync(HomePageQuery query, CancellationToken token)
        {
            return await
            _session.CreateSQLQuery("select top 1 [users], [answers], [SBLs], [money] from sb.HomeStats")
               .SetResultTransformer(Transformers.AliasToBean<StatsDto>())
               .UniqueResultAsync<StatsDto>();
             
        }
    }
}
