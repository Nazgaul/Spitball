using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class ShortUrlMap : ClassMap<ShortUrl>
    {
        public ShortUrlMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Destination).Length(800).Not.Nullable();
            Map(x => x.Identifier).Unique().Not.Nullable();
            Map(x => x.Expiration);
        }
    }
}