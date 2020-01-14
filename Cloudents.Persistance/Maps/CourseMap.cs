using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using FluentNHibernate.Mapping;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Fluent nhibernate")]
    public sealed class CourseMap : ClassMap<Course>
    {
        public CourseMap()
        {
            Id(e => e.Id).Column("Name").GeneratedBy.Assigned().Length(150);
            Map(x => x.Count).Not.Nullable();
            Map(x => x.Created).Insert().Not.Update();

            Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>();
            References(x => x.Subject).Column("SubjectId").Nullable();
            Map(x => x.SchoolType).CustomType<GenericEnumStringType<SchoolType>>().Nullable();
            HasMany(x => x.Users)
                .KeyColumn("CourseId").ForeignKeyConstraintName("Courses_User").Inverse().Cascade.AllDeleteOrphan().AsSet();
            DynamicUpdate();
            OptimisticLock.Version();
            Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();
        }
    }
}
