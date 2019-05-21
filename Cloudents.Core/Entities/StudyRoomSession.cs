using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Event;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor",Justification = "Nhibernate")]
    public class StudyRoomSession : Entity<Guid>
    {
        public StudyRoomSession(StudyRoom studyRoom,string sessionId)
        {
            StudyRoom = studyRoom;
            Created = DateTime.UtcNow;
            SessionId = sessionId;
            AddEvent(new StudyRoomSessionCreatedEvent(this));
        }
        protected StudyRoomSession()
        {

        }

        public virtual StudyRoom StudyRoom { get; protected set; }
        public virtual DateTime Created { get; protected set; }
        public virtual DateTime? Ended { get; protected set; }
        public virtual TimeSpan? Duration { get; protected set; }

        public virtual int RejoinCount { get; protected set; }
        public virtual string SessionId { get; protected set; }

        public virtual void EndSession()
        {
            if (Ended.HasValue)
            {
                throw new ArgumentException();
            }
            Ended = DateTime.UtcNow;
            Duration = Ended - Created;
        }

        public virtual void ReJoinStudyRoom()
        {
            RejoinCount++;
            AddEvent(new StudyRoomSessionRejoinEvent(this));
        }
    }
}
