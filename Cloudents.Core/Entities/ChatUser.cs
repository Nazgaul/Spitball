using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public class ChatUser : Entity<Guid>
    {
        protected ChatUser()
        {

        }

        public ChatUser(ChatRoom chatRoom, User user)
        {
            ChatRoom = chatRoom;
            User = user;
        }
        public virtual ChatRoom ChatRoom { get; protected set; }
        public virtual User User { get; protected set; }


        public virtual int Unread { get;  set; }


        public virtual byte[] Version { get; protected set; }

    }
}