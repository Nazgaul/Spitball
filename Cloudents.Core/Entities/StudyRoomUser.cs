using System;

namespace Cloudents.Core.Entities
{
    public class StudyRoomUser : Entity<Guid>
    {
        public StudyRoomUser(RegularUser user)
        {
            User = user;
        }

        protected StudyRoomUser()
        {

        }

        public virtual RegularUser User { get; protected set; }

        public virtual bool Online { get; set; }
    }
}