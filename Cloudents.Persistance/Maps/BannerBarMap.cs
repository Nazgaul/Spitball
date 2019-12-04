using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class BannerBarMap : ClassMap<BannerBar>
    {
        public BannerBarMap()
        {
            Id(x => x.Id);
            Map(x => x.EnTitle);
            Map(x => x.EnSubTitle);
            Map(x => x.HeTitle);
            Map(x => x.HeSubTitle);
            Map(x => x.EnInTitle);
            Map(x => x.EnInSubTitle);
            Map(x => x.ExpirationDate);
            References(x => x.Coupon).Cascade.None();
            ReadOnly();
        }
    }
}
