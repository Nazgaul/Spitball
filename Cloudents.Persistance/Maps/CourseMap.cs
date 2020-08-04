using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    //[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    //public sealed class OldCourseMap : ClassMap<OldCourse>
    //{
    //    public OldCourseMap()
    //    {
    //        Id(e => e.Id).Column("Name").GeneratedBy.Assigned().Length(150);
    //        Map(x => x.Count).Not.Nullable();
    //        Map(x => x.Created).Insert().Not.Update();
    //        DynamicUpdate();
    //        OptimisticLock.Version();
    //        Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();
    //        Table("Course");
    //    }
    //}

    public sealed class CourseMap : ClassMap<Course>
    {
        public CourseMap()
        {
            Id(x=>x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "5",
                $"{nameof(HiLoGenerator.TableName)}='Course'");
            Map(x => x.Name).Access.CamelCaseField(Prefix.Underscore).Not.Nullable();
            Map(x => x.Description).Access.CamelCaseField(Prefix.Underscore).Not.Nullable();
            Map(x => x.Position).ReadOnly();

            HasMany(x => x.Documents).Access.CamelCaseField(Prefix.Underscore)
                .Inverse().Cascade.AllDeleteOrphan();

            Map(x => x.Create);

            Map(x => x.SubscriptionPrice).Nullable()
                .CustomType<MoneyCompositeUserType>().Columns.Clear()
                .Columns.Add("SubscriptionPrice","SubscriptionCurrency");
            Map(x => x.Price)//.Not.Nullable()
                .CustomType<MoneyCompositeUserType>().Columns.Clear()
                .Columns.Add("Price","PriceCurrency");

            Map(x => x.StartTime).Access.CamelCaseField(Prefix.Underscore).Nullable();

            HasMany(x => x.StudyRooms).Access.CamelCaseField(Prefix.Underscore)
                .Inverse().Cascade.AllDeleteOrphan();


            HasMany(x => x.CourseEnrollments).Access.CamelCaseField(Prefix.Underscore)
                .Inverse().Cascade.AllDeleteOrphan().AsSet();

            References(x => x.Tutor).Not.Nullable();
            Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>();
            Table("Course2");
        }
    }
}
