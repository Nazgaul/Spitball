using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Extension;

//[assembly: InternalsVisibleTo("Cloudents.Persistance")]
namespace Cloudents.Core.Entities
{
    public class ChatRoom : AggregateRoot<Guid>
    {
        protected ChatRoom()
        {
            Users = new List<ChatUser>();
            Messages = new List<ChatMessage>();
        }

        public ChatRoom(IList<RegularUser> users)
        {
            Users = users.Select(s => new ChatUser(this, s)).ToList();
            Identifier = BuildChatRoomIdentifier(users.Select(s => s.Id));
            UpdateTime = DateTime.UtcNow;
        }

        public static string BuildChatRoomIdentifier(IEnumerable<long> userIds)
        {
            return string.Join("_", userIds.OrderBy(o => o));
        }

        public virtual DateTime UpdateTime { get; set; }

        public virtual ICollection<ChatUser> Users { get; protected set; }
        public virtual ICollection<ChatMessage> Messages { get; protected set; }

        public virtual string Identifier { get; set; }

        public virtual void AddMessage(ChatMessage message)
        {
            // var user = Users.Single(s => s.User.Id == userId);
            //var chatMessage = user.AddMessage(message, blob);

            //var chatMessage = new ChatMessage(user, message, blob, this);
            UpdateTime = DateTime.UtcNow;
            foreach (var otherUserInChat in Users.Where(s => s.User != message.User))
            {
                if (!otherUserInChat.User.Online)
                {
                    //TODO: need to send an email or something
                }
                otherUserInChat.Unread++;
            }
            AddEvent(new ChatMessageEvent(message));
            //return chatMessage;

            //this.AddEvent();
        }
    }

    public class ChatUser : Entity<Guid>
    {
        protected ChatUser()
        {

        }

        public ChatUser(ChatRoom chatRoom, RegularUser user)
        {
            ChatRoom = chatRoom;
            User = user;
        }

        // public virtual Guid Id { get; protected set; }
        public virtual ChatRoom ChatRoom { get; protected set; }
        public virtual RegularUser User { get; protected set; }


        public virtual int Unread { get; set; }

        //public virtual ChatMessage AddMessage(string message, string blob)
        //{
        //    var chatMessage = new ChatMessage(User, message, blob);
        //    Unread = 0;
        //    Messages.Add(chatMessage);

        //    return chatMessage;
        //}

    }

    public class ChatAttachmentMessage : ChatMessage
    {
        protected ChatAttachmentMessage()
        {

        }

        public ChatAttachmentMessage(RegularUser user, string blob, ChatRoom room) : base(user, room)
        {
            Blob = blob;

        }

        public virtual string Blob { get; protected set; }

    }

    public class ChatTextMessage : ChatMessage
    {
        protected ChatTextMessage()
        {

        }

        public ChatTextMessage(RegularUser user, string message, ChatRoom room) : base(user, room)
        {
            Message = message;

        }

        public virtual string Message { get; protected set; }

    }

    public abstract class ChatMessage : Entity<Guid>
    {
        protected ChatMessage()
        {

        }

       

        protected ChatMessage(RegularUser user, ChatRoom room)
        {
            User = user;
            CreationTime = DateTime.UtcNow;
            ChatRoom = room;
        }



        // public virtual Guid Id { get; protected set; }

        public virtual RegularUser User { get; protected set; }
        public virtual DateTime CreationTime { get; protected set; }

        public virtual ChatRoom ChatRoom { get; protected set; }

    }
}