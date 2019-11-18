using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("Cloudents.Persistence")]
namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class StudyRoom : Entity<Guid>, IAggregateRoot
    {
        public StudyRoom(Tutor tutor, User user, string onlineDocumentUrl)
        {

            _users = new[] { new StudyRoomUser(tutor.User, this), new StudyRoomUser(user, this) };
            Tutor = tutor;
            Identifier = ChatRoom.BuildChatRoomIdentifier(new[] { tutor.Id, user.Id });
            DateTime = DateTime.UtcNow;
            OnlineDocumentUrl = onlineDocumentUrl;
            Type = StudyRoomType.PeerToPeer;
            AddEvent(new StudyRoomCreatedEvent(this));
        }

        protected StudyRoom()
        {

        }

        public virtual Tutor Tutor { get; protected set; }

        private readonly ICollection<StudyRoomUser> _users = new List<StudyRoomUser>();

        public virtual IEnumerable<StudyRoomUser> Users => _users;


        public virtual string Identifier { get; protected set; }
        public virtual DateTime DateTime { get; protected set; }

        public virtual string OnlineDocumentUrl { get; protected set; }

        private readonly IList<StudyRoomSession> _sessions = new List<StudyRoomSession>();

        public virtual IEnumerable<StudyRoomSession> Sessions => _sessions;


        [CanBeNull]
        public virtual StudyRoomSession GetCurrentSession()
        {
            return Sessions.AsQueryable().SingleOrDefault(w => w.Ended == null);
        }

        public virtual StudyRoomType? Type { get; protected set; }

        public virtual void AddSession(StudyRoomSession session)
        {
            _sessions.Add(session);
        }

        public virtual void ChangeOnlineStatus(long userId, bool isOnline)
        {
            var studyRoomUser = Users.Single(f => f.User.Id == userId);
            studyRoomUser.ChangeOnlineState(isOnline);

        }

    }
}