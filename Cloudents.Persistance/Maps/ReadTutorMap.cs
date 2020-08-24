using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;


namespace Cloudents.Persistence.Maps
{
    public class ReadTutorMap : ClassMap<ReadTutor>
    {
        public ReadTutorMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Name);
            Map(x => x.ImageName);
            Map(x => x.Courses).CustomType<EnumerableJsonStringMapping>();
            Map(x => x.AllCourses).CustomType<EnumerableJsonStringMapping>();
            Map(x => x.Rate);
            Map(x => x.RateCount);
            Map(x => x.Bio).Length(1000);
            Map(x => x.Lessons);
            Map(x => x.OverAllRating).Column("Rating");
            Map(x => x.SbCountry).CustomType<EnumerationType<Country>>();
            Map(x => x.SubscriptionPrice).Nullable().CustomType<MoneyCompositeUserType>().Columns.Clear()
                .Columns.Add("SubscriptionPrice", "SubscriptionCurrency"); ;
            Map(x => x.Description);
            Map(x => x.State);

            Table("ReadTutor");
            DynamicUpdate();
        }
    }
}
