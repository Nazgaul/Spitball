using System;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.General
{
    public class CountryByIpQuery : IQuery<string?>
    {
        public CountryByIpQuery(string ip)
        {
            Ip = ip;
        }

        private string Ip { get; }

        internal sealed class CountryByIpQueryQueryHandler : IQueryHandler<CountryByIpQuery, string?>
        {
            private readonly IStatelessSession _session;

            public CountryByIpQueryQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<string?> GetAsync(CountryByIpQuery query, CancellationToken token)
            {

                using var c = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                using var source = CancellationTokenSource.CreateLinkedTokenSource(token, c.Token);
                try
                {
                    return await _session.Query<UserLocation>()
                        .WithOptions(w =>
                        {
                            w.SetComment(nameof(CountryByIpQuery));
                            w.SetTimeout(5);
                        })
                        .Fetch(f => f.User)
                        .Where(w => w.Ip == query.Ip && w.TimeStamp.CreationTime > DateTime.UtcNow.AddDays(-30))
                        .Select(s => s.Country)
                        .FirstOrDefaultAsync(source.Token);
                }
                catch (OperationCanceledException)
                {
                    return null;
                }
            }
        }
    }
}