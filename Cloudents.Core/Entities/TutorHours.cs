using System;

namespace Cloudents.Core.Entities
{
    public class TutorHours : Entity<Guid>
    {
        public TutorHours(Tutor tutor, DayOfWeek weekDay, TimeSpan from, TimeSpan to)
        {
            Tutor = tutor;
            WeekDay = weekDay;
            From = from;
            To = to;
            CreateTime = DateTime.UtcNow;
        }

        protected TutorHours()
        {
        }
        public virtual Tutor Tutor { get; protected set; }
        public virtual DayOfWeek WeekDay { get; protected set; }
        //public virtual IEnumerable<TimeFrame> TimeFrames { get; protected set; }
        public virtual TimeSpan From { get; protected set; }
        public virtual TimeSpan To { get; protected set; }

        public virtual DateTime CreateTime { get; protected set; }

        protected bool Equals(TutorHours other)
        {
            return Tutor.Id.Equals(other.Tutor.Id)
                && WeekDay.Equals(other.WeekDay)
                && From.Equals(other.From)
                && To.Equals(other.To);
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
            var hashCode = Tutor.Id.GetHashCode() ^ WeekDay.GetHashCode() ^ From.GetHashCode() ^ To.GetHashCode();
            return hashCode;
        }
    }

}
