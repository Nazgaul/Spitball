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


        protected bool Equals(ChatUser other)
        {

            return ChatRoom.Id.Equals(other.ChatRoom.Id) && User.Id.Equals(other.User.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ChatUser)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = ChatRoom.Id.GetHashCode() ^ User.Id.GetHashCode();
            return hashCode;
        }


        public virtual byte[] Version { get; protected set; }

    }
}