using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernane proxy")]

    public class TutorCalendar : Entity<Guid>
    {
        public TutorCalendar([NotNull] string googleId, [NotNull] string name, Tutor tutor)
        {
            GoogleId = googleId ?? throw new ArgumentNullException(nameof(googleId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Tutor = tutor;
        }

        protected TutorCalendar()
        {
        }


        public virtual string GoogleId { get; protected set; }
        public virtual string Name { get; protected set; }
        public virtual Tutor Tutor { get; protected set; }


        //protected bool Equals(TutorCalendar other)
        //{
        //    return base.Equals(other) && GoogleId == other.GoogleId && Tutor.Id.Equals(other.Tutor.Id);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    if (ReferenceEquals(this, obj)) return true;
        //    if (obj.GetType() != this.GetType()) return false;
        //    return Equals((TutorCalendar)obj);
        //}

        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        int hashCode = base.GetHashCode();
        //        hashCode = (hashCode * 397) ^ GoogleId.GetHashCode();
        //        hashCode = (hashCode * 397) ^ Tutor.Id.GetHashCode();
        //        return hashCode;
        //    }
        //}
        protected bool Equals(TutorCalendar other)
        {
            return string.Equals(GoogleId, other.GoogleId, StringComparison.OrdinalIgnoreCase) && Tutor.Id.Equals(other.Tutor.Id);
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

                var hashCode = StringComparer.OrdinalIgnoreCase.GetHashCode(GoogleId);
                hashCode = (hashCode * 397) ^ Tutor.Id.GetHashCode();
                return hashCode;
            }
        }
    }
}