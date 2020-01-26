using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.General
{
    public class ShortUrlQuery : IQuery<ShortUrlDto>
    {
        public ShortUrlQuery(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; private set; }


        internal sealed class ShortUrlQueryHandler : IQueryHandler<ShortUrlQuery, ShortUrlDto>
        {
            private readonly IStatelessSession _statelessSession;

            public ShortUrlQueryHandler(QuerySession statelessSession)
            {
                _statelessSession = statelessSession.StatelessSession;
            }


            public async Task<ShortUrlDto> GetAsync(ShortUrlQuery query, CancellationToken token)
            {
                return await _statelessSession.Query<ShortUrl>()
                      .Where(w => w.Identifier == query.Identifier)
                      .Where(w => w.Expiration.GetValueOrDefault(DateTime.MaxValue) > DateTime.UtcNow)
                      .Select(s => new ShortUrlDto()
                      {
                          Destination = s.Destination
                      }).SingleOrDefaultAsync(token);

            }
        }
    }

    public class ShortUrlDto
    {
        public string Destination { get; set; }
    }
}