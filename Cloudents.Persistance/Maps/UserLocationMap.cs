using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    internal class UserLocationMap : ClassMap<UserLocation>
    {
        public UserLocationMap()
        {
            Map();
        }

        private void Map()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User).Column("UserId").ForeignKey("UserLocation_User");
            Component(x => x.TimeStamp);
            Map(x => x.Ip).Nullable();
            Map(x => x.Country).Length(10).Nullable();
            //Map(x => x.FingerPrint).Nullable();
            Map(x => x.UserAgent).Nullable();
        }
    }
}