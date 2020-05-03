using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;

namespace Cloudents.Query.General
{
    public class ShortUrlQuery : IQuery<ShortUrlDto?>
    {
        public ShortUrlQuery(string identifier)
        {
            Identifier = identifier;
        }

        private string Identifier { get; }


        internal sealed class ShortUrlQueryHandler : IQueryHandler<ShortUrlQuery, ShortUrlDto?>
        {
            private readonly IStatelessSession _statelessSession;

            public ShortUrlQueryHandler(QuerySession statelessSession)
            {
                _statelessSession = statelessSession.StatelessSession;
            }


            public async Task<ShortUrlDto?> GetAsync(ShortUrlQuery query, CancellationToken token)
            {
                return await _statelessSession.Query<ShortUrl>()
                    .WithOptions(w => w.SetComment(nameof(ShortUrlQuery)))
                      .Where(w => w.Identifier == query.Identifier)
                      .Where(w => w.Expiration.GetValueOrDefault(DateTime.MaxValue) > DateTime.UtcNow)
                      .Select(s => new ShortUrlDto(s.Destination))
                    .SingleOrDefaultAsync(token);

            }
        }
    }

    
}