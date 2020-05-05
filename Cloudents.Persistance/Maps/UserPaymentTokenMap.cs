using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class UserPaymentTokenMap : ClassMap<UserPaymentToken>
    {
        public UserPaymentTokenMap()
        {

            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(m => m.OrderId).Column("TokenId").Not.Nullable();
            Map(m => m.AuthorizationId).Nullable(); 
            Map(m => m.Created).Insert().Not.Update();
            Map(m => m.Updated);
            Map(x => x.State).Not.Nullable();
            Map(x => x.Amount).CustomSqlType("Money");
            References(f => f.StudyRoom).Not.Nullable();
            References(f => f.StudyRoomSessionUser).Nullable();
            Table("UserToken");
        }
    }
}
