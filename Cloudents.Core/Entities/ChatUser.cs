using Cloudents.Core.Event;
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


        public virtual int Unread { get; protected set; }

        public virtual void ResetUnread()
        {
            Unread = 0;
            AddEvent(new ChatReadEvent(this));
        }

        public virtual void UnreadMessage()
        {
            Unread++;
        }


        public virtual byte[] Version { get; protected set; }

    }
}