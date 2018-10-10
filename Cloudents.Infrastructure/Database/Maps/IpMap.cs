using Cloudents.Core.Entities.Db;

namespace Cloudents.Infrastructure.Database.Maps
{
    public class IpMap :SpitballClassMap<Ip>
    {
        public IpMap()
        {
            Id(x => x.Id).GeneratedBy.Native();
            Map(x => x.From).Column("IP_FROM").CustomSqlType("numeric");
            Map(x => x.To).Column("IP_TO").CustomSqlType("numeric");
            Map(x => x.CountryCode).Column("COUNTRY_CODE2").CustomSqlType("nchar").Length(2);
            Map(x => x.CountryName).Column("COUNTRY_NAME").Length(50);
            ReadOnly();
            Table("IP_RANGE");
            Schema("Zbox");

            SchemaAction.None();
        }
    }
}