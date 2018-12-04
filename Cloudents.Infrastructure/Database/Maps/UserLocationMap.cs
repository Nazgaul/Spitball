using Cloudents.Core.Entities.Db;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Database.Maps
{
    internal class UserLocationMap : ClassMap<UserLocation>
    {
        public UserLocationMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User).Column("UserId").ForeignKey("UserLocation_User");
            Component(x => x.TimeStamp);
            Map(x => x.Ip).Nullable();
            Map(x => x.Country).Length(10).Nullable();
            SchemaAction.None();
        }
    }
}