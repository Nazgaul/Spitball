using Cloudents.Core.Enum;
using JetBrains.Annotations;
using System;

namespace Cloudents.Core.Entities
{
    public class ChatRoomAdmin : Entity<Guid>
    {
        protected ChatRoomAdmin()
        {

        }
        public ChatRoomAdmin(ChatRoom room) : this()
        {
            ChatRoom = room;
        }
        public virtual ChatRoomStatus Status { get; set; }
        public virtual string AssignTo { get; set; }

        public virtual ChatRoom ChatRoom { get; protected set; }


        [CanBeNull]
        public virtual Lead Lead { get; set; }
    }
}
