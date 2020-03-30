﻿using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class UserTokenMap : ClassMap<UserPayPalToken>
    {
        public UserTokenMap()
        {

            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(m => m.OrderId).Column("TokenId").Not.Nullable();
            Map(m => m.AuthorizationId).Not.Nullable();
            Map(m => m.Created).Insert().Not.Update();
            Map(m => m.Updated);
            Map(x => x.State).Not.Nullable();
            Map(x => x.Amount).CustomSqlType("Money");
            References(f => f.StudyRoom).Not.Nullable();
            Table("UserToken");
        }
    }
}
