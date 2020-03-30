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
        }
        protected StudyRoomSessionUser()
        {

        }
        public virtual StudyRoomSession StudyRoomSession { get; protected set; }


        public virtual User User { get; protected set; }

        public virtual TimeSpan? Duration { get; protected set; }


        public bool Equals(StudyRoomSessionUser other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && StudyRoomSession.Equals(other.StudyRoomSession) && User.Equals(other.User);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StudyRoomSessionUser) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ StudyRoomSession.GetHashCode();
                hashCode = (hashCode * 397) ^ User.GetHashCode();
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
