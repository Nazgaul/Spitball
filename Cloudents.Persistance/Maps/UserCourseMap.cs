using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public sealed class UserCourseMap : ClassMap<UserCourse>
    {
        public UserCourseMap()
        {
            CompositeId()
                .KeyReference(x => x.User, "UserId")
                .KeyReference(x => x.Course, "CourseId");
            Map(e => e.CanTeach).Not.Nullable();

            Table("UsersCourses");

            DynamicUpdate();
            OptimisticLock.Version();
            Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();

        }
    }
}