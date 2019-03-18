using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cloudents.Core.Event;

//[assembly: InternalsVisibleTo("Cloudents.Persistance")]
namespace Cloudents.Core.Entities
{
    public class ChatRoom : AggregateRoot<Guid>
    {
        protected ChatRoom()
        {

        }

        public ChatRoom(IEnumerable<RegularUser> users)
        {
            Users = users.Select(s => new ChatUser(this, s)).ToList();
            UpdateTime = DateTime.UtcNow;
        }

        public virtual DateTime UpdateTime { get; set; }

        public virtual ICollection<ChatUser> Users { get; protected set; }

        public virtual ChatMessage AddMessage(long userId, string message, string blob)
        {
            var user = Users.Single(s => s.User.Id == userId);
            var chatMessage = user.AddMessage(message, blob);
            UpdateTime = DateTime.UtcNow;
            foreach (var otherUserInChat in Users.Where(s => s.User.Id != userId))
            {
                if (!otherUserInChat.User.Online)
                {
                    //TODO: need to send an email or something
                }
                user.Unread++;
            }
            AddEvent(new ChatMessageEvent(chatMessage));
            return chatMessage;

            //this.AddEvent();
        }
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

        protected internal virtual ICollection<ChatMessage> Messages { get; set; }

        public virtual int Unread { get; set; }

        public virtual ChatMessage AddMessage(string message, string blob)
        {
            var chatMessage = new ChatMessage(this, message, blob);
            Unread = 0;
            Messages.Add(chatMessage);

            return chatMessage;
        }

    }

    public class ChatMessage
    {
        protected ChatMessage()
        {

        }

        public ChatMessage(ChatUser user, string message, string blob)
        {
            User = user;
            Message = message;
            CreationTime = DateTime.UtcNow;
            Blob = blob;

        }



        public virtual Guid Id { get; protected set; }

        public virtual ChatUser User { get; protected set; }
        public virtual string Message { get; protected set; }
        public virtual string Blob { get; protected set; }
        public virtual DateTime CreationTime { get; protected set; }

    }
}