﻿using Cloudents.Core.Event;
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
        [SuppressMessage("ReSharper", "CS8618", Justification = "Proxy")]
        [SuppressMessage("Code Quality", "CA8618")]
        protected ChatRoom()
        {
            Messages = new List<ChatMessage>();
            Users = new HashSet<ChatUser>();
            Extra = new ChatRoomAdmin(this);
            UpdateTime = DateTime.UtcNow;
        }

        public ChatRoom(IList<User> users, Tutor tutor)
        {
            foreach (var user in users)
            {
                user.AddFollowers(users);
            }

            Tutor = tutor;
            Users = new HashSet<ChatUser>(users.Select(s => new ChatUser(this, s)));
            Identifier = BuildChatRoomIdentifier(users.Select(s => s.Id));
            Messages = new List<ChatMessage>();
            Extra = new ChatRoomAdmin(this);
            UpdateTime = DateTime.UtcNow;
        }

        public static ChatRoom FromStudyRoom(StudyRoom studyRoom)
        {
            return new ChatRoom
            {
                Tutor = studyRoom.Tutor,
                Identifier = studyRoom.Identifier,
                StudyRoom = studyRoom
            };
        }

        public virtual Tutor Tutor { get; set; }

        public static string BuildChatRoomIdentifier(IEnumerable<long> userIds)
        {
            var userIdsList = userIds.Distinct().OrderBy(o => o).ToList();
            if (userIdsList.Count == 1)
            {
                throw new ArgumentException("need more then one participant");
            }
            return string.Join("_", userIdsList);
        }

        public static string BuildChatRoomIdentifier(params long[] userIds)
        {
            var userIdsList = userIds.Distinct().OrderBy(o => o).ToList();
            if (userIdsList.Count == 1)
            {
                throw new ArgumentException("need more then one participant");
            }
            return string.Join("_", userIdsList);
        }

        public static IEnumerable<long> IdentifierToUserIds(string identifier)
        {
            return identifier.Split("_").Select(long.Parse);
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
            Messages.Add(message);
            AddEvent(new ChatMessageEvent(message));
            if (StudyRoom != null)
            {
                if (StudyRoom is BroadCastStudyRoom _)
                {
                    return;
                }
            }

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