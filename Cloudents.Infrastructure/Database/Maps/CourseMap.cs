using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;

namespace Cloudents.Infrastructure.Database.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global",Justification = "Fluent nhibernate")]
    public sealed class CourseMap : SpitballClassMap<Course>
    {
        public CourseMap()
        {
            Id(e => e.Name).GeneratedBy.Assigned().Length(150);
            Map(x => x.Count).Not.Nullable();

            SchemaAction.Update();
        }
    }
}
