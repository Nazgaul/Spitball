using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Domain.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Course
    {
        public const int MinLength = 4;
        public const int MaxLength = 150;
        protected Course()
        {

        }

        public Course(string name)
        {
            Name = name.Trim();//.Replace("+", string.Empty);
            if (Name.Length > MaxLength || Name.Length < MinLength)
            {
                throw new ArgumentException();
            }
        }

        protected bool Equals(Course other)
        {
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Course)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Name) : 0);
        }

        public static bool operator ==(Course left, Course right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Course left, Course right)
        {
            return !Equals(left, right);
        }

        public virtual string Name { get; protected set; }
        public virtual int Count { get; set; }
    }
}
