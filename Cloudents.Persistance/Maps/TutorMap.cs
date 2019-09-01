﻿using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;
using Cloudents.Core.Enum;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public class TutorMap : ClassMap<Tutor>
    {
        public TutorMap()
        {
            Id(x => x.Id).GeneratedBy.Foreign("User");
            //CompositeId()
            //    .KeyReference(x => x.User, "UserId");
            //Id(x => x.UserId).GeneratedBy.Assigned();
            HasOne(x => x.User).Constrained().Cascade.None();
            Map(x => x.Bio).Length(1000);
            Map(x => x.Price).CustomSqlType("smallMoney");

            Map(x => x.SellerKey);
            HasMany(x => x.Reviews).Access.CamelCaseField(Prefix.Underscore).Cascade.AllDeleteOrphan().Inverse();
            Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>();
            Map(e => e.Created).Insert().Not.Update();
            Map(x => x.ManualBoost).LazyLoad().Nullable();
            DynamicUpdate();
            OptimisticLock.Version();
            Version(x => x.Version).CustomSqlType("rowversion").Generated.Always();
        }
    }
}