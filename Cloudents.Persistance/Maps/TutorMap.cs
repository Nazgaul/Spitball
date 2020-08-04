using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class TutorMap : ClassMap<Tutor>
    {
        public TutorMap()
        {
            Id(x => x.Id).GeneratedBy.Foreign("User");
           
            HasOne(x => x.User).Constrained().Cascade.None();
            

            Map(x => x.Title).Nullable();
            Map(x => x.Paragraph3).Nullable().Length(8000);
            Map(x => x.Paragraph2).Nullable().Column("Bio").Length(1000);

            Map(x => x.SubscriptionPrice).Nullable()
                .CustomType<MoneyCompositeUserType>().Columns.Clear()
                .Columns.Add("SubscriptionPrice","SubscriptionCurrency");
            Map(x => x.SellerKey);
            HasMany(x => x.Reviews).Access.CamelCaseField(Prefix.Underscore).Cascade.AllDeleteOrphan().Inverse();
            HasMany(x => x.Calendars).Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan().Inverse().AsSet();
            HasMany(x => x.TutorHours).Access.CamelCaseField(Prefix.Underscore)
                .Inverse().Cascade.AllDeleteOrphan().AsSet();

            HasMany(x => x.StudyRooms)
                .Cascade.AllDeleteOrphan().Inverse();

            HasMany(x => x.Leads)
                .Cascade.AllDeleteOrphan().Inverse();


            HasMany(x => x.Coupons)
                .Cascade.AllDeleteOrphan().Inverse();

            //no inverse so we can have position persist in db - i know extra update
            HasMany(x => x.Courses)
                .Cascade.AllDeleteOrphan().AsList(x =>
                {
                    x.Type<int>();
                    x.Column("Position");
                });

            HasMany(x => x.UserCoupons)
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan().Inverse();

            Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>();
            Map(e => e.Created).Insert().Not.Update();
            //Map(x => x.ManualBoost).LazyLoad().Nullable();
          //  Map(e => e.IsShownHomePage);
            HasMany(x => x.AdminUsers).Inverse().Cascade.AllDeleteOrphan();
            DynamicUpdate();
            OptimisticLock.Version();
            Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();

        }
    }

   
}