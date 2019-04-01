using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

//[assembly: InternalsVisibleTo("Cloudents.Persistance")]
namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
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
            foreach (var usersInChat in Users)
            {
                if (usersInChat.User != message.User)
                {

                    if (!usersInChat.User.Online)
                    {
                        //TODO: need to send an email or something
                    }

                    usersInChat.Unread++;
                }
                else
                {
                    usersInChat.Unread = 0;
                }
            }
            AddEvent(new ChatMessageEvent(message));
            //return chatMessage;

            //this.AddEvent();
        }
    }
}