using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Mapping")]
    public sealed class UserCourse2Map : ClassMap<UserCourse2>
    {
        public UserCourse2Map()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(e => e.IsTeach).Column("CanTeach").Not.Nullable();
            References(x => x.User).Not.Nullable().UniqueKey("k-UserCourse2");
            References(x => x.Course).Not.Nullable().UniqueKey("k-UserCourse2");
        }
    }
}