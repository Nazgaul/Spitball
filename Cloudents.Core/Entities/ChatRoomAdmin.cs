using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Entities
{
    public class ChatRoomAdmin : Entity<Guid>
    {
        public ChatRoomAdmin() { }
        public ChatRoomAdmin(ChatRoomStatus status, string assignTo)
        {
            Status = status;
            AssignTo = assignTo;
        }
        public virtual ChatRoomStatus Status { get; set; }
        public virtual string AssignTo { get; set; }
    }
}
