using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;

namespace Cloudents.Core.Entities
{
    public class BroadCastStudyRoom : StudyRoom
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public BroadCastStudyRoom(Tutor tutor,
            string onlineDocumentUrl,
            Course course, DateTime broadcastTime, string description)
            : base(tutor, Enumerable.Empty<User>(), onlineDocumentUrl, 0)
        {
            Identifier = Guid.NewGuid().ToString();
            ChatRoom = ChatRoom.FromStudyRoom(this);
            BroadcastTime = broadcastTime;
            TopologyType = StudyRoomTopologyType.GroupRoom;
            Course = course ?? throw new ArgumentNullException(nameof(course));
            Description = description;
        }

        public virtual string Description { get; set; }

        protected BroadCastStudyRoom() : base()
        {
        }

        public virtual DateTime BroadcastTime { get; set; }




        [Obsolete]
        public virtual StudyRoomSchedule? Schedule { get; protected set; }

        public virtual Course Course { get; protected set; }

     

        public override void AddUserToStudyRoom(User user)
        {
            ChatRoom.AddUserToChat(user);
            if (Tutor.Id == user.Id)
            {
                return;
            }
            var studyRoomUser = new StudyRoomUser(user, this);

            var x = _users.Add(studyRoomUser);
            if (x)
            {
                Tutor.User.AddFollower(user);
                AddEvent(new AddUserBroadcastStudyRoomEvent(this, user));
            }
        }




        public override StudyRoomType Type => StudyRoomType.Broadcast;
    }
}