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
    public class Course //: Entity<string>
    {
        public const int MinLength = 4;
        public const int MaxLength = 150;
        protected Course()
        {
            Tutors = new List<TutorsCourses>();
        }

        public Course(string name)
        {
            Name = name.Trim();//.Replace("+", string.Empty);
            if (Name.Length > MaxLength || Name.Length < MinLength)
            {
                throw new ArgumentException($"Name is {Name}",nameof(Name));
            }

            State = ItemState.Pending;

            Created = DateTime.UtcNow;

            Tutors = new List<TutorsCourses>();
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

        public virtual void Approve()
        {
            //TODO: maybe put an event to that
            if (State == ItemState.Pending)
            {
                State = ItemState.Ok;
            }
        }

        public virtual string Name { get; protected set; }
        public virtual int Count { get; set; }

        public virtual DateTime Created { get;protected set; }

        private readonly ISet<RegularUser> _users = new HashSet<RegularUser>();
        public virtual IReadOnlyCollection<RegularUser> Users => _users.ToList();


        private readonly IList<Document> _documents = new List<Document>();
        public virtual IReadOnlyList<Document> Documents => _documents.ToList();

        private readonly IList<Question> _questions = new List<Question>();
        public virtual IReadOnlyList<Question> Questions => _questions.ToList();


        protected internal virtual IList<TutorsCourses> Tutors { get; set; }
        
  
        public virtual ItemState State { get; protected set; }
    }
}
