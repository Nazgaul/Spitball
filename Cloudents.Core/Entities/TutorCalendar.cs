using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernane proxy")]

    public class TutorCalendar : Entity<Guid>
    {
        public TutorCalendar(GoogleCalendar calendar, Tutor tutor)
        {
            Calendar = calendar;
            Tutor = tutor;
        }

        protected TutorCalendar()
        {
        }

        public virtual GoogleCalendar Calendar { get; protected set; }

        public virtual Tutor Tutor { get; protected set; }


       
        protected bool Equals(TutorCalendar other)
        {
            return Calendar.Equals(other.Calendar) &&
                   Tutor.Id.Equals(other.Tutor.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TutorCalendar)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {

                var hashCode = Calendar.GetHashCode();
                hashCode = (hashCode * 397) ^ Tutor.Id.GetHashCode();
                return hashCode;
            }
        }
    }

    public class GoogleCalendar : ValueObject
    {
        protected GoogleCalendar()
        {
            
        }
        public GoogleCalendar(string googleId, string name)
        {
            GoogleId = googleId;
            Name = name;
        }

        public string GoogleId { get; protected set; }
        public string Name { get; protected set; }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return GoogleId;
        }
    }
}