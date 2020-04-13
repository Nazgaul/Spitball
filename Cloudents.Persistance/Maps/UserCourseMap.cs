using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public sealed class UserCourseMap : ClassMap<UserCourse>
    {
        public UserCourseMap()
        {
            CompositeId()
                .KeyReference(x => x.User, "UserId")
                .KeyReference(x => x.Course, "CourseId");
            Map(e => e.IsTeach).Column("CanTeach").Not.Nullable();

            Table("UsersCourses");

            DynamicUpdate();
            OptimisticLock.Version();
            Version(x => x.Version).CustomSqlType("timestamp").Generated.Always();

        }
    }


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