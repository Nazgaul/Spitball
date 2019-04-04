using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Cloudents.Persistence")]
namespace Cloudents.Core.Entities
{
    public class StudyRoom : AggregateRoot<Guid>
    {
        public StudyRoom(Tutor tutor, RegularUser user, string onlineDocumentUrl)
        {
            Tutor = tutor;
            Users = new[] { new StudyRoomUser(user) };
            Identifier = ChatRoom.BuildChatRoomIdentifier(new[] { tutor.Id, user.Id });
            DateTime = DateTime.UtcNow;
            OnlineDocumentUrl = onlineDocumentUrl;
        }

        protected StudyRoom()
        {
            
        }

        public virtual Tutor Tutor { get; protected set; }
        protected internal virtual ICollection<StudyRoomUser> Users { get; set; }
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


    public class StudyRoomUser : Entity<Guid>
    {
        public StudyRoomUser(RegularUser user)
        {
            User = user;
        }

        protected StudyRoomUser()
        {

        }

        public virtual RegularUser User { get; set; }
    }
}