using System;

namespace Cloudents.Core
{
    public struct TutorDailyHours
    {
        public TutorDailyHours(DayOfWeek day, TimeSpan @from, TimeSpan to)
        {
            Day = day;
            From = @from;
            To = to;
        }

        public bool Equals(TutorDailyHours other)
        {
            return Day == other.Day && From.Equals(other.From) && To.Equals(other.To);
        }

        public override bool Equals(object obj)
        {
            return obj is TutorDailyHours other && Equals(other);
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

        public TimeSpan From { get; }
        public TimeSpan To { get; }

        public static bool operator ==(TutorDailyHours left, TutorDailyHours right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TutorDailyHours left, TutorDailyHours right)
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
