using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities.Db
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class UserCourseRelationship
    {
        protected UserCourseRelationship()
        {
            CreateTime = DateTime.UtcNow;
            
        }

        public UserCourseRelationship(User user, Course course) : this()
        {
            User = user;
            Course = course;
        }

        public virtual Guid Id { get; set; }

        public virtual User User { get; set; }
        public virtual Course Course { get; set; }


        public virtual DateTime CreateTime { get; protected set; }


        public override bool Equals(object obj)
        {
            if (!(obj is UserCourseRelationship t))
                return false;
            if (User.Id == t.User.Id && Course.Id == t.Course.Id)
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            return (User.Id + "|" + Course.Id).GetHashCode();
        }
    }
}