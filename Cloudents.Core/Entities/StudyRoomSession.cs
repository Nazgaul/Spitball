using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class StudyRoomSession : Entity<Guid>
    {
        public StudyRoomSession(StudyRoom studyRoom, string sessionId)
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

        //TODO remove this
        public virtual TimeSpan? DurationInMinutes { get; protected set; }

        public virtual int RejoinCount { get; protected set; }
        public virtual string SessionId { get; protected set; }
        public virtual string Receipt { get; protected set; }
        public virtual decimal? Price { get; protected set; }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "nhibernate proxy")]
        public virtual byte[] Version { get; protected set; }

        private readonly IList<SessionParticipantDisconnect> _participantDisconnections = new List<SessionParticipantDisconnect>();

        public virtual IEnumerable<SessionParticipantDisconnect> ParticipantDisconnections => _participantDisconnections;

        public virtual void EndSession()
        {
            if (Ended.HasValue)
            {
                return;
            }
            Ended = DateTime.UtcNow;
            Duration = Ended - Created;
            DurationInMinutes = Ended - Created;
            if (StudyRoom.Tutor.Price.SubsidizedPrice == null)
            {
                Price = ((decimal)Math.Floor(DurationInMinutes.Value.TotalMinutes) / 60) * StudyRoom.Tutor.Price.Price;
            }
            else
            {
                Price = ((decimal)Math.Floor(DurationInMinutes.Value.TotalMinutes) / 60) * StudyRoom.Tutor.Price.SubsidizedPrice;
            }
                
            AddEvent(new EndStudyRoomSessionEvent(this));
        }

        public virtual void ReJoinStudyRoom()
        {
            RejoinCount++;
            AddEvent(new StudyRoomSessionRejoinEvent(this));
        }

        public virtual void SetReceipt(string receipt)
        {
            if (string.IsNullOrEmpty(receipt))
            {
                throw new ArgumentException();
            }
            Receipt = receipt;
        }
    }
}
