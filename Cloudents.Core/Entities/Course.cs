using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
            Tutors = new List<TutorsCourses>();
        }
        
        public Course(string name)
        {
            Id = name.Trim();//.Replace("+", string.Empty);
            if (Id.Length > MaxLength || Id.Length < MinLength)
            {
                throw new ArgumentException($"Name is {Id}",nameof(Id));
            }

            State = ItemState.Pending;

            Created = DateTime.UtcNow;

            Tutors = new List<TutorsCourses>();
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

        public override int GetHashCode()
        {
            return (Id != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Id) : 0);
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

        //public virtual string Name { get; protected set; }
        public virtual int Count { get; set; }

        public virtual DateTime Created { get;protected set; }

        private readonly ISet<UserCourse> _users = new HashSet<UserCourse>();
        public virtual IReadOnlyCollection<UserCourse> Users => _users.ToList();


        //private readonly IList<Document> _documents = new List<Document>();
        //public virtual IReadOnlyList<Document> Documents => _documents.ToList();

        //private readonly IList<Question> _questions = new List<Question>();
        //public virtual IReadOnlyList<Question> Questions => _questions.ToList();


        protected internal virtual IList<TutorsCourses> Tutors { get; set; }
        
  
        public virtual ItemState State { get; protected set; }
    }

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
