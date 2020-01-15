using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernane proxy")]
    public class TutorHours : Entity<Guid>
    {
        public TutorHours(Tutor tutor, TutorAvailabilitySlot availabilitySlot)
        {
            Tutor = tutor;
            AvailabilitySlot = availabilitySlot;
            CreateTime = DateTime.UtcNow;
        }

        protected TutorHours()
        {
        }

        public virtual Tutor Tutor { get; protected set; }
        public virtual TutorAvailabilitySlot AvailabilitySlot { get; protected set; }

        public virtual DateTime CreateTime { get; protected set; }

        protected bool Equals(TutorHours other)
        {
            return Tutor.Id.Equals(other.Tutor.Id)
                && AvailabilitySlot.Equals(other.AvailabilitySlot);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TutorHours)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = Tutor.Id.GetHashCode() ^ AvailabilitySlot.GetHashCode();
            return hashCode;
        }
    }

}
