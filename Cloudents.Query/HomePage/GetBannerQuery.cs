using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.HomePage
{
    public class GetBannerQuery : IQuery<BannerDto>
    {
        private CultureInfo Culture { get; }
        public GetBannerQuery(CultureInfo culture)
        {
            Culture = culture;
        }


        internal sealed class GetBannerQueryHandler : IQueryHandler<GetBannerQuery, BannerDto>
        {
            private readonly IStatelessSession _session;
            private readonly ICacheProvider _cacheProvider;

            public GetBannerQueryHandler(QuerySession session, ICacheProvider cacheProvider)
            {
                _cacheProvider = cacheProvider;
                _session = session.StatelessSession;
            }
            public async Task<BannerDto> GetAsync(GetBannerQuery query, CancellationToken token)
            {
                var v = _cacheProvider.Get(query.Culture.Name, "Banner");
                if (v is null)
                {
                    var res = await _session.Query<BannerBar>().Where(w => w.ExpirationDate > DateTime.UtcNow)
                        .Where(w => query.Culture.Equals(new CultureInfo("he")) ? w.HeTitle != null :
                            query.Culture.Equals(new CultureInfo("en-in")) ? w.EnInTitle != null : w.EnTitle != null
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

                    if (res is null)
                    {
                        _cacheProvider.Set(query.Culture.Name, "Banner", "no", TimeSpan.FromMinutes(30), false);
                    }
                    return res;
                }

                return null;
                //var res = _session.Query<BannerBar>().Where(w => w.ExpirationDate > DateTime.UtcNow)
                //    .Where(w => query.Culture.Equals(new CultureInfo("he")) ? w.HeTitle != null :
                //     query.Culture.Equals(new CultureInfo("en-in"))? w.EnInTitle != null : w.EnTitle != null
                //    ).Select(s => new BannerDto()
                //{ 
                //Id = s.Id,
                //Title = query.Culture.Equals(new CultureInfo("he")) ? s.HeTitle :
                //query.Culture.Equals(new CultureInfo("en-in")) ? s.EnInTitle : s.EnTitle,
                //SubTitle = query.Culture.Equals(new CultureInfo("he")) ? s.HeSubTitle :
                //query.Culture.Equals(new CultureInfo("en-in")) ? s.EnInSubTitle : s.EnSubTitle,
                //ExpirationDate = s.ExpirationDate,
                //Coupon = s.Coupon.Code
                //    })
                //    .OrderBy(o => o.ExpirationDate).Take(1)
                //.SingleOrDefaultAsync(token);

                //return res;
            }
        }
    }
}
