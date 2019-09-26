using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using JetBrains.Annotations;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public class UserLoginsMap : ClassMapping<UserLogin>
    {
        public UserLoginsMap()
        {
            ComposedId(x => {
                x.Property(p => p.LoginProvider);
                x.Property(p => p.ProviderKey);
            });
            //CompositeId().KeyProperty(x => x.LoginProvider).KeyProperty(x => x.ProviderKey);
            Property(x => x.ProviderDisplayName, c => c.NotNullable(false));
            //Map(x => x.ProviderDisplayName).Nullable();
            ManyToOne(x => x.User, c => {
                c.Column("UserId");
                c.ForeignKey("UserLogin_User");
            });
            //References(x => x.User).Column("UserId").ForeignKey("UserLogin_User");
            Table("UserLogin");
            //Table("UserLogin");
            SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.Validate);
            //SchemaAction.Validate();
        }
    }
}