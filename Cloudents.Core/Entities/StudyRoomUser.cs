using Cloudents.Core.Event;
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

        public virtual bool Online { get; protected set; }

        public virtual void ChangeOnlineState(bool isOnline)
        {
            Online = isOnline;
            //AddEvent(new StudyRoomOnlineChangeEvent(this));
        }
    }
}