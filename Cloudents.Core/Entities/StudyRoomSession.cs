﻿using System;

namespace Cloudents.Core.Entities
{
    public class StudyRoomSession : Entity<Guid>
    {
        public StudyRoomSession(StudyRoom studyRoom,string sessionId)
        {
            StudyRoom = studyRoom;
            Created = DateTime.UtcNow;
            SessionId = sessionId;
        }
        protected StudyRoomSession()
        {

        }

        public virtual StudyRoom StudyRoom { get; protected set; }
        public virtual DateTime Created { get; protected set; }
        public virtual DateTime? Ended { get; protected set; }
        public virtual TimeSpan? Duration { get; protected set; }

        public virtual string SessionId { get; protected set; }

        public void EndSession()
        {
            Ended = DateTime.UtcNow;
            Duration = Ended - Created;
        }
    }
}
