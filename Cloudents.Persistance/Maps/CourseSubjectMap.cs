using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using JetBrains.Annotations;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public sealed class CourseSubjectMap : ClassMapping<CourseSubject>
    {
        public CourseSubjectMap()
        {
            Id(x => x.Id, c => c.Generator(Generators.Identity));
            //Id(x => x.Id).GeneratedBy.Identity();
            Property(x => x.Name, c => {
                c.NotNullable(true);
                c.Unique(true);
                c.Length(150);
            });
            //Map(e => e.Name).Not.Nullable().Unique().Length(150);
            DynamicUpdate(true);
            //DynamicUpdate();
            OptimisticLock(OptimisticLockMode.Version);
            //OptimisticLock.Version();
            Version(x => x.Version, c => {
                c.Generated(VersionGeneration.Always);
                c.Column(cl => {
                    cl.SqlType("timestamp");
                });
            });
            //Version(x => x.Version).CustomSqlType("rowversion").Generated.Always();
            Mutable(false);
            //ReadOnly();
        }
    }
}
