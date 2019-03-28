using System;

namespace Cloudents.Core.Entities
{
    public class UserCourse : IEquatable<UserCourse>
    {
        public virtual  RegularUser User { get;protected set; }
        public virtual  Course Course { get; protected set; }

        protected UserCourse()
        {
            
        }

        public UserCourse(RegularUser user, Course course)
        {
            User = user;
            Course = course;
        }

        public virtual bool Equals(UserCourse other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(User, other.User) && Equals(Course, other.Course);
        }

        public  override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserCourse) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((User != null ? User.GetHashCode() : 0) * 397) ^ (Course != null ? Course.GetHashCode() : 0);
            }
        }

        public static bool operator ==(UserCourse left, UserCourse right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UserCourse left, UserCourse right)
        {
            return !Equals(left, right);
        }
    }
}