using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace Cloudents.Persistence.Maps
{
    public class ReadTutorMap : ClassMapping<ReadTutor>
    {
        public ReadTutorMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.Assigned));
            //Id(x => x.Id).GeneratedBy.Assigned();
            Property(x => x.Name);
            //Map(x => x.Name);
            Property(x => x.Image);
            //Map(x => x.Image);
            Property(x => x.Subjects, c => c.Type<StringAggMapping>());
            //Map(x => x.Subjects).CustomType<StringAggMapping>();
            Property(x => x.AllSubjects, c => {
                c.Column(cl => cl.SqlType("nvarchar(max)"));
                c.Type<StringAggMapping>();
            });
            //Map(x => x.AllSubjects).CustomType<StringAggMapping>().CustomSqlType("nvarchar(max)");
            Property(x => x.Courses, c => c.Type<StringAggMapping>());
            //Map(x => x.Courses).CustomType<StringAggMapping>();
            Property(x => x.AllCourses, c => {
                c.Type<StringAggMapping>();
                c.Column(cl => cl.SqlType("nvarchar(max)"));
            });
            //Map(x => x.AllCourses).CustomType<StringAggMapping>().CustomSqlType("nvarchar(max)");
            Property(x => x.Price, c => c.Column(cl => cl.SqlType("smallmoney")));
            //Map(x => x.Price);
            Property(x => x.Rate);
            //Map(x => x.Rate);
            Property(x => x.RateCount);
            //Map(x => x.RateCount);
            Property(x => x.Bio, c => c.Length(1000));
            //Map(x => x.Bio).Length(1000);
            Property(x => x.University);
            //Map(x => x.University);
            Property(x => x.Lessons);
            //Map(x => x.Lessons);
            Property(x => x.Rating);
            //Map(x => x.Rating);
            Table("ReadTutor");
            //Table("ReadTutor");
            //// SchemaAction.Update();
        }
    }
}
