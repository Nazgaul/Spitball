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

            Table("UsersCourses");

        }
    }
}