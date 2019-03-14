using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Course //: Entity<string>
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
                throw new ArgumentException($"Name is {Name}",nameof(Name));
            }

            Created = DateTime.UtcNow;
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

        public virtual DateTime Created { get; set; }

        public virtual ISet<RegularUser> Users { get; protected set; }
        public virtual IList<Document> Documents { get; protected set; }
        public virtual IList<Question> Questions { get; protected set; }
        public virtual ItemState State { get; protected set; }
    }
}
