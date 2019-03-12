using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Core.Entities
{
    public class ChatRoom
    {
        protected ChatRoom()
        {

        }

        public ChatRoom(IEnumerable<RegularUser> users)
        {
            Users = users.Select(s => new ChatUser(this, s)).ToList();
            UpdateTime = DateTime.UtcNow;
        }

        public virtual Guid Id { get; protected set; }
        public virtual DateTime UpdateTime { get; set; }

        public virtual ICollection<ChatUser> Users { get; protected set; }
    }

    public class ChatUser
    {
        protected ChatUser()
        {

        }

        public ChatUser(ChatRoom chatRoom, RegularUser user)
        {
            ChatRoom = chatRoom;
            User = user;
        }

        public virtual Guid Id { get; protected set; }
        public virtual ChatRoom ChatRoom { get; protected set; }
        public virtual RegularUser User { get; protected set; }

        public virtual int Unread { get; set; }
    }

    public class ChatMessage
    {
        protected ChatMessage()
        {

        }

        public ChatMessage(ChatUser user, string message)
        {
            User = user;
            Message = message;
            CreationTime = DateTime.UtcNow;
        }



        public virtual Guid Id { get; protected set; }

        public virtual ChatUser User { get; protected set; }
        public virtual string Message { get; protected set; }
        public virtual string Blob { get; protected set; }
        public virtual DateTime CreationTime { get; protected set; }

    }
}