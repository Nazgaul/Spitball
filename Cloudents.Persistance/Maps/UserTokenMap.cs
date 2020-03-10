using System.Data;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace Cloudents.Persistence.Maps
{
    public class UserTokenMap : ClassMap<UserPayPalToken>
    {
        public UserTokenMap()
        {

            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(m => m.TokenId).Not.Nullable();
            Map(m => m.Created).Insert().Not.Update();
            Map(x => x.State).Not.Nullable();
            Map(x => x.Amount).CustomSqlType("Money");
            Table("UserToken");
        }
    }
}
