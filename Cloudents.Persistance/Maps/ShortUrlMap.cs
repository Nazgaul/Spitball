using Cloudents.Core.Entities;
using NHibernate.Mapping.ByCode;
//using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;

namespace Cloudents.Persistence.Maps
{
    public class ShortUrlMap : ClassMapping<ShortUrl>
    {
        public ShortUrlMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.GuidComb)); //.GeneratedBy.GuidComb();
            Property(x => x.Destination, c => { c.Length(800); c.NotNullable(true); });  //Map(x => x.Destination).Length(800).Not.Nullable();
            Property(x => x.Identifier, c => { c.Unique(true); c.NotNullable(true); }); //Map(x => x.Identifier).Unique().Not.Nullable();
            Property(x => x.Expiration); //Map(x => x.Expiration);
        }
    }
}