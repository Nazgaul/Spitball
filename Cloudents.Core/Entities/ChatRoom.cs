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
            Extra = new ChatRoomAdmin(this);
        }

        public ChatRoom(IList<User> users) : this()
        {

            Users = users.Select(s => new ChatUser(this, s)).ToList();
            Identifier = BuildChatRoomIdentifier(users.Select(s => s.Id));
            TimeStamp = new DomainTimeStamp();

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

        public virtual DomainTimeStamp TimeStamp { get; protected set; }

        public virtual ICollection<ChatUser> Users { get; protected set; }
        public virtual ICollection<ChatMessage> Messages { get; protected set; }

        public virtual string Identifier { get; protected set; }
        public virtual ChatRoomAdmin Extra { get; set; }

        public virtual void AddMessage(ChatMessage message)
        {
            TimeStamp.UpdateTime = DateTime.UtcNow;
            foreach (var userInChat in Users)
            {
                if (userInChat.User.Id != message.User.Id)
                {
                    userInChat.UnreadMessage();
                }
                else
                {
                    userInChat.ResetUnread();
                }
            }

            //Status = ChatRoomStatus.Default;
            AddEvent(new ChatMessageEvent(message));
        }

    }
}