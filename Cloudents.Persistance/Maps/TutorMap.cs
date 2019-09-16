using Cloudents.Core.Entities;
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
            HasMany(x => x.Calendars).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan().Inverse().AsSet();
            HasMany(x => x.TutorHours).Access.CamelCaseField(Prefix.Underscore)
                .Inverse().Cascade.All().AsSet();

            HasMany(x => x.StudyRooms).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan().Inverse().AsSet();

            Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>();
            Map(e => e.Created).Insert().Not.Update();
            Map(x => x.ManualBoost).LazyLoad().Nullable();
            DynamicUpdate();
            OptimisticLock.Version();
            Version(x => x.Version).CustomSqlType("rowversion").Generated.Always();
        }

        public class TutorCalendarMap : ClassMap<TutorCalendar>
        {
            public TutorCalendarMap()
            {
                Id(x => x.Id);
                Map(x => x.Name).Not.Nullable();
                Map(x => x.GoogleId).Not.Nullable();

                References(x => x.Tutor).Not.Nullable();
            }
        }
    }
}