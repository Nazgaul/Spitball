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
            Map(x => x.Image);
            Map(x => x.Subjects).CustomType<IEnumerableJsonStringMapping>();
            Map(x => x.AllSubjects).CustomType<IEnumerableJsonStringMapping>().CustomSqlType("nvarchar(max)");
            Map(x => x.Courses).CustomType<IEnumerableJsonStringMapping>();
            Map(x => x.AllCourses).CustomType<IEnumerableJsonStringMapping>().CustomSqlType("nvarchar(max)");
            Map(x => x.Price);
            Map(x => x.Rate);
            Map(x => x.RateCount);
            Map(x => x.Bio).Length(1000);
            Map(x => x.University);
            Map(x => x.Lessons);
            Map(x => x.OverAllRating).Column("Rating");
            Table("ReadTutor");
            DynamicUpdate();
           // SchemaAction.Update();
        }
    }
}
