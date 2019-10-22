﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Models;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query
{
    public class CountryByIpQuery : IQuery<string>
    {
        public CountryByIpQuery(string ip)
        {
            Ip = ip;
        }

        private string Ip { get; }

        internal sealed class CountryByIpQueryQueryHandler : IQueryHandler<CountryByIpQuery, string>
        {
            private readonly IStatelessSession _session;

            public CountryByIpQueryQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<string> GetAsync(CountryByIpQuery query, CancellationToken token)
            {
                return await _session.Query<UserLocation>()
                    .Fetch(f => f.User)
                    .Where(w => w.Ip == query.Ip).Select(s => s.Country )
                    .FirstOrDefaultAsync(token);
            }
        }
    }
}