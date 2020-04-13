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
            References(x => x.Subject).Column("SubjectId").Nullable().ForeignKey("FK_1152B92");
            Map(x => x.SchoolType).CustomType<GenericEnumStringType<SchoolType>>().Nullable();
            HasMany(x => x.Users)
                .KeyColumn("CourseId").ForeignKeyConstraintName("Courses_User").Inverse().Cascade.AllDeleteOrphan().AsSet();
            Map(x => x.Country);
            DynamicUpdate();
            OptimisticLock.Version();
            Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();
        }
    }

    public sealed class Course2Map: ClassMap<Course2>
    {
        public Course2Map()
        {
            Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "3", $"{nameof(HiLoGenerator.TableName)}='{nameof(Course2)}'");
            Map(x => x.Count).Not.Nullable();
            Map(x => x.Created).Insert().Not.Update();
            Map(x => x.State).CustomType<GenericEnumStringType<ItemState>>();
            Map(x => x.Field).Not.Nullable().UniqueKey("K-Course2Restriction");
            Map(x => x.Country).UniqueKey("K-Course2Restriction").CustomType<EnumerationType<Country>>().Not.Nullable(); 
            Map(x => x.Subject).UniqueKey("K-Course2Restriction").Not.Nullable();
            Map(x => x.SearchDisplay);
            Map(x => x.CardDisplay);

            HasMany(x => x.Users)
                .KeyColumn("CourseId")
                .Inverse().Cascade.AllDeleteOrphan().AsSet();
        }

    }
}
