using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Mapping")]
    public sealed class DocumentCourseMap : ClassMap<DocumentCourse>
    {
        public DocumentCourseMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.Document).Not.Nullable().UniqueKey("k-DocumentCourse2");
            References(x => x.Course).Not.Nullable().UniqueKey("k-DocumentCourse2");
        }
    }
}