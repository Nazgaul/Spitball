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
            Id(e => e.Name).GeneratedBy.Assigned();
            Map(x => x.Count).Not.Nullable();
            Map(x => x.Extra).Length(4000).LazyLoad();

            SchemaAction.Update();
        }
    }
}
