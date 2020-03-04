using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cloudents.Persistence")]
namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class Course : Entity<string>
    {
        public const int MinLength = 4;
        public const int MaxLength = 150;
        protected Course()
        {
        }

        public Course(string name)
        {
            Id = name.Trim();//.Replace("+", string.Empty);
            if (Id.Length > MaxLength || Id.Length < MinLength)
            {
                throw new ArgumentException($"Name is {Id}", nameof(Id));
            }
            State = ItemState.Pending;
            Created = DateTime.UtcNow;
        }

        protected bool Equals(Course other)
        {
            return string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Course)obj);
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "Nhibernate")]
        public override int GetHashCode()
        {
            return Id != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Id) : 0;
        }


        public static bool operator ==(Course left, Course right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Course left, Course right)
        {
            return !Equals(left, right);
        }

        public virtual void Approve()
        {
            //TODO: maybe put an event to that
            if (State == ItemState.Pending)
            {
                State = ItemState.Ok;
            }
        }

        public virtual void SetSubject(CourseSubject subject)
        {
            Subject = subject;
        }

        public virtual void SetShcoolType(SchoolType schoolType)
        {
            SchoolType = schoolType;
        }
        public virtual int Count { get; protected internal set; }



        public virtual DateTime Created { get; protected set; }

        //private readonly ISet<UserCourse> _users = new HashSet<UserCourse>();
        //public virtual IEnumerable<UserCourse> Users => _users;

        protected internal virtual ISet<UserCourse> Users { get; set; }


        public virtual ItemState State { get; protected set; }
        public virtual CourseSubject Subject { get; protected set; }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "nhibernate proxy")]
        public virtual byte[] Version { get; protected set; }
        public virtual SchoolType? SchoolType { get; protected set; }
        public virtual string Country { get; protected set; }
    }
}
