using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.Enum;

//[assembly: InternalsVisibleTo("Cloudents.Persistance")]
namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    public class ChatRoom : Entity<Guid>, IAggregateRoot
    {
        protected ChatRoom()
        {
            Users ??= new HashSet<ChatUser>();
            Messages = new List<ChatMessage>();
            Extra = new ChatRoomAdmin(this);
            UpdateTime = DateTime.UtcNow;

        }

        public ChatRoom(IList<User> users) : this()
        {
            foreach (var user in users)
            {
                user.AddFollowers(users);
            }
            Users = new HashSet<ChatUser>(users.Select(s => new ChatUser(this, s)));
            Identifier = BuildChatRoomIdentifier(users.Select(s => s.Id));
        }

        internal static ChatRoom FromStudyRoom(StudyRoom studyRoom)
        {
            return new ChatRoom
            {
                Identifier = studyRoom.Identifier,
                StudyRoom = studyRoom
            };
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

        public virtual StudyRoom? StudyRoom { get; protected set; }

        public virtual DateTime UpdateTime { get; protected set; }

        public virtual ISet<ChatUser> Users { get; protected set; }
        public virtual ICollection<ChatMessage> Messages { get; protected set; }

        public virtual string Identifier { get; protected set; }
        public virtual ChatRoomAdmin Extra { get; set; }

        public virtual void AddTextMessage(User user, string message)
        {
            var chatMessage = new ChatTextMessage(user, message, this);
            AddMessage(chatMessage);
        }

        public virtual void AddMessage(ChatMessage message)
        {
            UpdateTime = DateTime.UtcNow;
            if (StudyRoom != null)
            {
                if (StudyRoom is PrivateStudyRoom _) 
                {
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
                }
            }

            Messages.Add(message);
            AddEvent(new ChatMessageEvent(message));
        }

        public virtual void AddUserToChat(User user)
        {
            if (StudyRoom != null)
            {
                if (StudyRoom is BroadCastStudyRoom _)
                {
                    var chatUser = new ChatUser(this, user);
                    Users.Add(chatUser);
                }

                //if (StudyRoom?.StudyRoomType == StudyRoomType.Broadcast
                //) // we update unread only on not broadcast studyroom
                //{
                   
                //}
            }
        }

    }
}