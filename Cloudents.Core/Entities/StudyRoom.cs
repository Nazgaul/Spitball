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
            OnlineDocumentUrl = onlineDocumentUrl;
            Type = StudyRoomType.PeerToPeer;
            AddEvent(new StudyRoomCreatedEvent(this));
        }

        protected StudyRoom()
        {
            DateTime = new DomainTimeStamp();
        }

        public virtual Tutor Tutor { get; protected set; }

        private readonly ICollection<StudyRoomUser> _users = new List<StudyRoomUser>();

        public virtual IEnumerable<StudyRoomUser> Users => _users;


        public virtual string Identifier { get; protected set; }
        public virtual DomainTimeStamp DateTime { get; protected set; }

        public virtual string OnlineDocumentUrl { get; protected set; }

        private readonly IList<StudyRoomSession> _sessions = new List<StudyRoomSession>();

        public virtual IEnumerable<StudyRoomSession> Sessions => _sessions;


        [CanBeNull]
        public virtual StudyRoomSession GetCurrentSession()
        {

            var result = Sessions.AsQueryable().Where(w => w.Ended == null).OrderBy(o => o.Id).ToList();

            //if (result.Count > 1)
            //{
            for (int i = 0; i < result.Count - 1; i++)
            {
                result[i].EndSession();
            }

            //}
            return result.SingleOrDefault(w => w.Ended == null);
        }

        public virtual StudyRoomType? Type { get; protected set; }

        public virtual void AddSession(StudyRoomSession session)
        {
            _sessions.Add(session);
            DateTime.UpdateTime = System.DateTime.UtcNow;
        }

        public virtual void ChangeOnlineStatus(long userId, bool isOnline)
        {
            var studyRoomUser = Users.Single(f => f.User.Id == userId);
            studyRoomUser.ChangeOnlineState(isOnline);

        }

    }
}