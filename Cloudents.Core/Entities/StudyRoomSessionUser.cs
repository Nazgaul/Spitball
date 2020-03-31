using Cloudents.Core.Event;
using System;

namespace Cloudents.Core.Entities
{
    public class StudyRoomSessionUser : Entity<Guid>, IEquatable<StudyRoomSessionUser>
    {
        public StudyRoomSessionUser(StudyRoomSession studyRoomSession, User user)
        {
            StudyRoomSession = studyRoomSession;
            User = user;
            PricePerHour = studyRoomSession.StudyRoom.Tutor.Price.GetPrice();

        }
        protected StudyRoomSessionUser()
        {

        }
        public virtual StudyRoomSession StudyRoomSession { get; protected set; }


        public virtual User User { get; protected set; }

        public virtual TimeSpan? Duration { get; protected set; }

        public virtual int DisconnectCount { get; protected set; }

        public virtual decimal PricePerHour { get; protected set; }

        public virtual TimeSpan? TutorApproveTime { get; protected set; }

        public virtual decimal TotalPrice { get; protected set; }

        public virtual string? Receipt { get; set; }

        public virtual void Disconnect(TimeSpan durationInRoom)
        {
            Duration = Duration.GetValueOrDefault(TimeSpan.Zero) + durationInRoom;
            TotalPrice = (decimal)Duration.Value.TotalHours * PricePerHour;
            DisconnectCount++;
        }

        public virtual void ApproveSession(TimeSpan duration)
        {
            TutorApproveTime = duration;
            TotalPrice = (decimal)TutorApproveTime.Value.TotalHours * PricePerHour;
        }


        public virtual bool Equals(StudyRoomSessionUser other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StudyRoomSession.Id.Equals(other.StudyRoomSession.Id) && User.Id.Equals(other.User.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StudyRoomSessionUser)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = StudyRoomSession.Id.GetHashCode();
                hashCode = (hashCode * 397) ^ User.Id.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(StudyRoomSessionUser left, StudyRoomSessionUser right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StudyRoomSessionUser left, StudyRoomSessionUser right)
        {
            return !Equals(left, right);
        }
    }
}
