using System;
using Cloudents.Core.Event;

namespace Cloudents.Core.Entities
{
    public class StudyRoomUser : Entity<Guid>
    {
        public StudyRoomUser(RegularUser user,StudyRoom room)
        {
            User = user;
            Room = room;
        }

        protected StudyRoomUser()
        {
            
        }

        public virtual RegularUser User { get; protected set; }
        public virtual StudyRoom Room { get; protected set; }

        public virtual bool Online { get; protected set; }

        public virtual void ChangeOnlineState(bool isOnline)
        {
            Online = isOnline;
            AddEvent(new StudyRoomOnlineChangeEvent(this));
        }
    }
}