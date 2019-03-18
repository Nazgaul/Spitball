using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Fluent nhibernate")]
    public sealed class CourseMap : ClassMap<Course>
    {
        public CourseMap()
        {
            Table("Course2");
            Id(x => x.Id).GeneratedBy.HiLo(nameof(HiLoGenerator), nameof(HiLoGenerator.NextHi), "10",
                $"{nameof(HiLoGenerator.TableName)}='{nameof(Course)}'");
            Map(e => e.Name).Not.Nullable().Length(150);
            Map(x => x.Count).Not.Nullable();
            Map(x => x.Created);
            
            SchemaAction.None();
        }
    }
}
