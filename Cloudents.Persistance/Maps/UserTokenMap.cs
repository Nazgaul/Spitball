using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class UserTokenMap : ClassMap<UserPayPalToken>
    {
        public UserTokenMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            //References(r => r.User).Column("UserId");
            Map(m => m.TokenId).Not.Nullable();
            Map(m => m.Created).Insert().Not.Update();
            Map(x => x.State).Not.Nullable();

            Table("UserToken");
        }
    }
}
