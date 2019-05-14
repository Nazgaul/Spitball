using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    internal class UserLoginsMap : ClassMap<UserLogin>
    {
        public UserLoginsMap()
        {
            CompositeId().KeyProperty(x => x.LoginProvider).KeyProperty(x => x.ProviderKey);
            Map(x => x.ProviderDisplayName).Nullable();
            References(x => x.User).Column("UserId").ForeignKey("UserLogin_User");
            Table("UserLogin");
            SchemaAction.Validate();
        }
    }
}