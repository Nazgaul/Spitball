using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;

namespace Cloudents.Core.Entities
{
    public class BroadCastStudyRoom : StudyRoom
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public BroadCastStudyRoom(Tutor tutor,
            IEnumerable<User> users, string onlineDocumentUrl,
            string name, decimal price, DateTime broadcastTime, string? description)
            : base(tutor, users, onlineDocumentUrl, name, price)
        {
            Identifier = Guid.NewGuid().ToString();
            ChatRoom = ChatRoom.FromStudyRoom(this);
            BroadcastTime = broadcastTime;
            TopologyType = StudyRoomTopologyType.GroupRoom;
            Description = description;
        }

        protected BroadCastStudyRoom() : base()
        {
        }

        public virtual DateTime BroadcastTime { get; protected set; }


        public virtual string? Description { get; protected set; }

        public override void AddUserToStudyRoom(User user)
        {
            if (Tutor.Id == user.Id)
            {
                return;
            }
            var studyRoomUser = new StudyRoomUser(user, this);
           var z =  _users.Add(studyRoomUser);
            ChatRoom.AddUserToChat(user);
             Tutor.User.AddFollower(user);
            AddEvent(new AddUserBroadcastStudyRoomEvent(this, user));
        }



        public override StudyRoomType Type => StudyRoomType.Broadcast;
    }
}