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
            Course course, decimal price, DateTime broadcastTime, string? description, StudyRoomSchedule? schedule)
            : base(tutor, Enumerable.Empty<User>(), onlineDocumentUrl, price)
        {
            Identifier = Guid.NewGuid().ToString();
            ChatRoom = ChatRoom.FromStudyRoom(this);
            BroadcastTime = broadcastTime;
            TopologyType = StudyRoomTopologyType.GroupRoom;
            Description = description;
            Schedule = schedule;
            Course = course ?? throw new ArgumentNullException(nameof(course));
        }

        protected BroadCastStudyRoom() : base()
        {
        }

        public virtual DateTime BroadcastTime { get;  set; }


        public virtual string? Description { get; protected set; }

        public virtual StudyRoomSchedule? Schedule { get; protected set; }

        public virtual Course Course { get; set; }

        public virtual void AddPayment(User user,string receipt)
        {
            var studyRoomPayment = new StudyRoomPayment(this, user, receipt);
            _studyRoomPayments.Add(studyRoomPayment);
        }

        public override void AddUserToStudyRoom(User user)
        {
            ChatRoom.AddUserToChat(user);
            if (Tutor.Id == user.Id)
            {
                return;
            }
            var studyRoomUser = new StudyRoomUser(user, this);
            
            _users.Add(studyRoomUser);
           
             Tutor.User.AddFollower(user);
            AddEvent(new AddUserBroadcastStudyRoomEvent(this, user));
        }




        public override StudyRoomType Type => StudyRoomType.Broadcast;
    }
}