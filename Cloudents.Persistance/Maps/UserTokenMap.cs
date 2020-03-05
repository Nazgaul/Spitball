using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class UserTokenMap : ClassMap<UserToken>
    {
        public UserTokenMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(r => r.User).Column("UserId");
            Map(m => m.TokenId);
            Map(m => m.Created).Insert().Not.Update();
        }
    }
}
