using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{

    public sealed class CourseMap : ClassMap<Course>
    {
        public CourseMap()
        {
            DynamicUpdate();
            Id(x=>x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "5",
                $"{nameof(HiLoGenerator.TableName)}='Course'");
            Map(x => x.Name).Access.CamelCaseField(Prefix.Underscore).Not.Nullable();
            Map(x => x.Description).Access.CamelCaseField(Prefix.Underscore).Not.Nullable();
            Map(x => x.Position).ReadOnly();

            HasMany(x => x.Documents).Access.CamelCaseField(Prefix.Underscore)
                .Inverse().Cascade.AllDeleteOrphan();

            Component(x => x.DomainTime, z =>
            {
                z.Map(c => c.CreationTime).Column("Create").Insert().Not.Update();
                z.Map(c => c.UpdateTime);
            });

            Map(x => x.SubscriptionPrice).Nullable()
                .CustomType<MoneyCompositeUserType>().Columns.Clear()
                .Columns.Add("SubscriptionPrice","SubscriptionCurrency");
            Map(x => x.Price)//.Not.Nullable()
                .CustomType<MoneyCompositeUserType>().Columns.Clear()
                .Columns.Add("Price","PriceCurrency");

            Map(x => x.StartTime).Access.CamelCaseField(Prefix.Underscore).Nullable();

            HasMany(x => x.StudyRooms).Access.CamelCaseField(Prefix.Underscore)
                .Inverse().Cascade.AllDeleteOrphan().AsSet();


            HasMany(x => x.CourseEnrollments).Access.CamelCaseField(Prefix.Underscore)
                .Inverse().Cascade.AllDeleteOrphan().AsSet();

            References(x => x.Tutor).Not.Nullable();
            Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>();
            Version(x => x.Version).Nullable();
            Table("Course2");
        }
    }
}
