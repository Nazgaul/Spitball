using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public sealed class CourseSubjectMap : ClassMap<CourseSubject>
    {
        public CourseSubjectMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(e => e.Name).Not.Nullable().Unique().Length(150);
            //Map(e => e.EnglishName).Not.Nullable().Unique().Length(150).Column("Name_en");
            HasMany(x => x.Translations).Access.CamelCaseField(Prefix.Underscore)
               .Cascade.AllDeleteOrphan()
               .KeyColumn("SubjectId").Inverse().AsSet();
            DynamicUpdate();
            OptimisticLock.Version();
            Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();
        }
    }
}
