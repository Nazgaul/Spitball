using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities
{
    public class ChatRoomAdmin : Entity<Guid>
    {
        protected ChatRoomAdmin() { }
        public ChatRoomAdmin(ChatRoomStatus status, Lead lead)
        {
            Status = status;
            Lead = lead;
        }
        public virtual ChatRoomStatus Status { get; set; }
        public virtual ChatRoomAssign AssignTo { get; set; }

        [CanBeNull]
        public virtual Lead Lead { get; protected set; }
    }
}
