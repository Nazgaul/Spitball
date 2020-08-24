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
        public BroadCastStudyRoom(
             Course course, DateTime broadcastTime, string description)
            : base(course.Tutor, Enumerable.Empty<User>(),  0)
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


        protected bool Equals(BroadCastStudyRoom other)
        {
            return BroadcastTime.Equals(other.BroadcastTime) && Course.Id.Equals(other.Course.Id);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BroadCastStudyRoom) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(37, Course.Id, BroadcastTime);
        }
    }
}