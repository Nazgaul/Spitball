using Cloudents.Core.Enum;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernane proxy")]

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


        public virtual Lead? Lead { get; set; }
    }
}
