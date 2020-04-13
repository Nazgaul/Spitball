using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    public class UserCourse2 :Entity<Guid>, IEquatable<UserCourse2>
    {
        protected UserCourse2()
        {

        }

        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public UserCourse2(User user, Course2 course)
        {
            User = user;
            IsTeach = user.Tutor != null;
            Course = course;
        }

        public virtual User User { get; protected set; }
        public virtual Course2 Course { get; protected set; }

        public virtual bool IsTeach { get; protected set; }

        public virtual void CanTeach(bool canTeach)
        {
            IsTeach = canTeach;
            //TODO - COURSE-CLEANUP

           // _domainEvents.Add(new CanTeachCourseEvent(this));
        }

        public virtual bool Equals(UserCourse2? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return User.Id.Equals(other.User.Id) && Course.Id.Equals(other.Course.Id);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserCourse2) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(User.Id, Course.Id);
        }
    }
}