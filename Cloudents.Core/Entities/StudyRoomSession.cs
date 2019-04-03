using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Entities
{
    public class StudyRoomSession : Entity<Guid>
    {
        public StudyRoomSession(StudyRoom studyRoom, DateTime created, DateTime? ended, TimeSpan? duration)
        {
            StudyRoom = studyRoom;
            Created = created;
            Ended = ended;
            Duration = duration;
        }
        protected StudyRoomSession()
        {

        }

        public virtual StudyRoom StudyRoom { get; protected set; }
        public virtual DateTime Created { get; protected set; }
        public virtual DateTime? Ended { get; protected set; }
        public virtual TimeSpan? Duration { get; protected set; }

        public static void EndSession(StudyRoomSession session)
        {
            session.Ended = DateTime.UtcNow;
            session.Duration = session.Ended - session.Created;
        }
    }
}
