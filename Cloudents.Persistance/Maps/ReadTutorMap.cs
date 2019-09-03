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
            Map(x => x.Subjects).CustomType<StringAggMapping>();
            Map(x => x.AllSubjects).CustomType<StringAggMapping>().CustomSqlType("nvarchar(max)");
            Map(x => x.Courses).CustomType<StringAggMapping>();
            Map(x => x.AllCourses).CustomType<StringAggMapping>().CustomSqlType("nvarchar(max)");
            Map(x => x.Price).CustomSqlType("smallMoney");
            Map(x => x.SubsidizedPrice).CustomSqlType("smallMoney");
            Map(x => x.Rate);
            Map(x => x.RateCount);
            Map(x => x.Bio).Length(1000);
            Map(x => x.University);
            Map(x => x.Lessons);
            Map(x => x.Rating);
            Table("ReadTutor");
            SchemaAction.Update();
        }
    }
}
