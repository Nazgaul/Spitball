using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]
    public class StudyRoomUser : Entity<Guid>
    {
        public StudyRoomUser(User user, StudyRoom room)
        {
            User = user;
            Room = room;
        }

        protected StudyRoomUser()
        {

        }

        public virtual User User { get; protected set; }
        public virtual StudyRoom Room { get; protected set; }


        protected bool Equals(StudyRoomUser other)
        {

            return Room.Id.Equals(other.Room.Id) && User.Id.Equals(other.User.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StudyRoomUser)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = Room.Id.GetHashCode() ^ User.Id.GetHashCode();
            return hashCode;
        }
    }
}