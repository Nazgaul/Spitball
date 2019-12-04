using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query
{
    public class GetBannerQuery : IQuery<BannerDto>
    {
        public CultureInfo Culture { get; set; }
        public GetBannerQuery(CultureInfo culture)
        {
            Culture = culture;
        }


        internal sealed class GetBannerQueryHandler : IQueryHandler<GetBannerQuery, BannerDto>
        {
            private readonly IStatelessSession _session;

            public GetBannerQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            public Task<BannerDto> GetAsync(GetBannerQuery query, CancellationToken token)
            {
                var res = _session.Query<BannerBar>().Where(w => w.ExpirationDate > DateTime.UtcNow)
                    .Where(w => query.Culture.Equals(new CultureInfo("he")) ? w.HeTitle != null :
                     query.Culture.Equals(new CultureInfo("en-in"))? w.EnInTitle != null : w.EnTitle != null
                    ).Select(s => new BannerDto()
                { 
                Id = s.Id,
                Title = query.Culture.Equals(new CultureInfo("he")) ? s.HeTitle :
                query.Culture.Equals(new CultureInfo("en-in")) ? s.EnInTitle : s.EnTitle,
                SubTitle = query.Culture.Equals(new CultureInfo("he")) ? s.HeSubTitle :
                query.Culture.Equals(new CultureInfo("en-in")) ? s.EnInSubTitle : s.EnSubTitle,
                ExpirationDate = s.ExpirationDate,
                Coupon = s.Coupon.Code
                    })
                    .OrderBy(o => o.ExpirationDate).Take(1)
                .SingleOrDefaultAsync(token);

                return res;
            }
        }
    }
}
