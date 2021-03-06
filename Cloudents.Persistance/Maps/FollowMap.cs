﻿using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class FollowMap : ClassMap<Follow>
    {
        public FollowMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User).Column("UserId").Not.Nullable().UniqueKey("c_follower");
            References(x => x.Follower).Column("FollowerId").Not.Nullable().UniqueKey("c_follower");
            Map(x => x.Created).Insert().Not.Update();
            Map(x => x.Subscriber).Nullable();
            Table("UsersRelationship");
        }
    }
}
