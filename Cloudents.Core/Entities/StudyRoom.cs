using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Cloudents.Core.Event;

[assembly:InternalsVisibleTo("Cloudents.Persistence")]
namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor",Justification = "Nhibernate")]
    public class StudyRoom : AggregateRoot<Guid>
    {
        public StudyRoom(Tutor tutor, RegularUser user, string onlineDocumentUrl)
        {
            
            _users = new[] { new StudyRoomUser(tutor.User),  new StudyRoomUser(user) };
            Identifier = ChatRoom.BuildChatRoomIdentifier(new[] { tutor.Id, user.Id });
            DateTime = DateTime.UtcNow;
            OnlineDocumentUrl = onlineDocumentUrl;
            AddEvent(new StudyRoomCreatedEvent(this));
        }

        protected StudyRoom()
        {
            
        }

        //public virtual Tutor Tutor { get; protected set; }

        private readonly ICollection<StudyRoomUser> _users = new List<StudyRoomUser>();

        public virtual IReadOnlyList<StudyRoomUser> Users => _users.ToList();


        public virtual string Identifier { get; protected set; }
        public virtual DateTime DateTime { get; protected set; }

        public virtual string OnlineDocumentUrl { get; protected set; }

        private readonly IList<StudyRoomSession> _sessions = new List<StudyRoomSession>();

        public virtual IReadOnlyList<StudyRoomSession> Sessions => _sessions.ToList();

        public virtual void AddSession(StudyRoomSession session)
        {
            _sessions.Add(session);
        }
        
    }
}