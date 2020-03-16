using System;

namespace Cloudents.Core
{
    public struct TutorAvailabilitySlot
    {
        public TutorAvailabilitySlot(DayOfWeek day, DateTimeOffset @from, DateTimeOffset to)
        {
            Day = day;
            From = @from;
            To = to;
        }

        public bool Equals(TutorAvailabilitySlot other)
        {
            return Day == other.Day && From.Equals(other.From) && To.Equals(other.To);
        }

        public override bool Equals(object obj)
        {
            return obj is TutorAvailabilitySlot other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Day;
                hashCode = (hashCode * 397) ^ From.GetHashCode();
                hashCode = (hashCode * 397) ^ To.GetHashCode();
                return hashCode;
            }
        }

        public DayOfWeek Day { get; }

        public DateTimeOffset From { get; }
        public DateTimeOffset To { get; }

        public static bool operator ==(TutorAvailabilitySlot left, TutorAvailabilitySlot right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TutorAvailabilitySlot left, TutorAvailabilitySlot right)
        {
            return !(left == right);
        }
        // public IList<TimeSpan> TimeFrames { get; protected set; }
        //protected override IEnumerable<object> GetEqualityComponents()
        //{
        //    yield return Day;
        //    yield return TimeFrames;
        //}
    }
}
