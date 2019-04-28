using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

//[assembly: InternalsVisibleTo("Cloudents.Persistance")]
namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public class ChatRoom : Entity<Guid>, IAggregateRoot
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
            var userIdsList = userIds.Distinct().OrderBy(o => o).ToList();
            if (userIdsList.Count == 1)
            {
                throw new ArgumentException("need more then one participant");
            }
            return string.Join("_", userIdsList);
        }

        public virtual DateTime UpdateTime { get; set; }

        public virtual ICollection<ChatUser> Users { get; protected set; }
        public virtual ICollection<ChatMessage> Messages { get; protected set; }

        public virtual string Identifier { get; set; }

        public virtual void AddMessage(ChatMessage message)
        {
            UpdateTime = DateTime.UtcNow;
            foreach (var userInChat in Users)
            {
                if (userInChat.User != message.User)
                {
                    userInChat.Unread++;
                    if (!userInChat.User.Online && userInChat.Unread < 2)
                    {
                        AddEvent(new OfflineChatMessageEvent(userInChat));
                    }
                }
                else
                {
                    userInChat.Unread = 0;
                }
            }
            AddEvent(new ChatMessageEvent(message));
        }
    }
}