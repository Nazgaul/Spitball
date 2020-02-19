﻿using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

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
            Component(x => x.Price, y2 =>
            {
                y2.Map(z => z.Price).CustomSqlType("smallMoney");
                y2.Map(z => z.SubsidizedPrice).CustomSqlType("smallMoney");
            });


            Map(x => x.SellerKey);
            HasMany(x => x.Reviews).Access.CamelCaseField(Prefix.Underscore).Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.Calendars).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan().Inverse().AsSet();
            HasMany(x => x.TutorHours).Access.CamelCaseField(Prefix.Underscore)
                .Inverse().Cascade.AllDeleteOrphan().AsSet();

            HasMany(x => x.StudyRooms)/*.Access.CamelCaseField(Prefix.Underscore)*/
                .Cascade.AllDeleteOrphan().Inverse().AsSet();

            HasMany(x => x.Leads)
                .Cascade.AllDeleteOrphan().Inverse().AsSet();

            Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>();
            Map(e => e.Created).Insert().Not.Update();
            Map(x => x.ManualBoost).LazyLoad().Nullable();
            Map(e => e.IsShownHomePage);
            HasMany(x => x.AdminUsers).Inverse().Cascade.AllDeleteOrphan();
            DynamicUpdate();
            OptimisticLock.Version();
            Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();

        }
    }
}