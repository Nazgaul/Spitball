using System;

namespace Cloudents.Core.Entities
{
    public class CourseEnrollment : Entity<Guid>
    {
        public CourseEnrollment(User user, Course course, string? receipt, Money price)
        {
            User = user;
            Course = course;
            Create = DateTime.UtcNow;
            Receipt = receipt;
            Price = price;
        }

        protected CourseEnrollment()
        {
            
        }

        public virtual User User { get; set; }

        protected bool Equals(CourseEnrollment other)
        {
            return User.Id.Equals(other.User.Id) && Course.Id.Equals(other.Course.Id);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CourseEnrollment) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(23, User.Id, Course.Id);
        }

        public static bool operator ==(CourseEnrollment? left, CourseEnrollment? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CourseEnrollment? left, CourseEnrollment? right)
        {
            return !Equals(left, right);
        }

        public virtual Course Course { get;  }
        public virtual string? Receipt { get;  }


        public virtual DateTime Create { get;  }
        public virtual Money? Price { get;  }

        //Coupon
        //recepit
    }
}