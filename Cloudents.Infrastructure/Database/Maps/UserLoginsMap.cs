using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Database.Maps
{
    [UsedImplicitly]
    internal class UserLoginsMap : SpitballClassMap<UserLogin>
    {
        public UserLoginsMap()
        {
            CompositeId().KeyProperty(x => x.LoginProvider).KeyProperty(x => x.ProviderKey);
            Map(x => x.ProviderDisplayName).Nullable();
            References(x => x.User).Column("UserId").ForeignKey("UserLogin_User");
            Table("UserLogin");
            SchemaAction.None();
        }
    }

    internal class UserLocationMap : SpitballClassMap<UserLocation>
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