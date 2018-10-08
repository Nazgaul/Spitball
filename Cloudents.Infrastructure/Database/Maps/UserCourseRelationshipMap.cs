using Cloudents.Core.Entities.Db;

namespace Cloudents.Infrastructure.Database.Maps
{
    public class UserCourseRelationshipMap : SpitballClassMap<UserCourseRelationship>
    {
        private const string UniqueConstraint = "uniqueRelationship";
        public UserCourseRelationshipMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User).Column("UserId").ForeignKey("UserCourseRelationship_User").Not.Nullable().UniqueKey(UniqueConstraint);
            References(x => x.Course).Column("CourseId").ForeignKey("UserCourseRelationship_Course").Not.Nullable().UniqueKey(UniqueConstraint);
        }
    }
}