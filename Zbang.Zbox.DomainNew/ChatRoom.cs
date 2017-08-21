using System;
using System.Linq;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.IdGenerator;

namespace Zbang.Zbox.Domain
{
    public class ChatRoom
    {
        protected ChatRoom()
        {

        }

        public ChatRoom(IEnumerable<User> users)
        {
            Id = GuidIdGenerator.GetGuid(); ;
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

        public ChatUser(ChatRoom chatRoom , User user)
        {
            Id = GuidIdGenerator.GetGuid();
            ChatRoom = chatRoom;
            User = user;
        }

        public virtual Guid Id { get; protected set; }
        public virtual ChatRoom ChatRoom { get; protected set; }
        public virtual User User { get; protected set; }

        public virtual int Unread { get; set; }
    }

    public class ChatMessage
    {
        protected ChatMessage()
        {

        }

        public ChatMessage(ChatRoom chatRoom, User user, string message, string blob)
        {
            Id = GuidIdGenerator.GetGuid();
            ChatRoom = chatRoom;
            User = user;
            Message = message;
            CreationTime = DateTime.UtcNow;
            Blob = blob;
        }

        public virtual Guid Id { get; protected set; }

        public virtual ChatRoom ChatRoom { get; protected set; }
        public virtual User User { get; protected set; }
        public virtual string Message { get; protected set; }
        public virtual DateTime CreationTime { get; protected set; }

        public virtual string Blob { get; protected set; }
    }
}
