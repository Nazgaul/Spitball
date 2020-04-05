using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cloudents.Persistence")]
namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class StudyRoom : Entity<Guid>, IAggregateRoot
    {
        public StudyRoom(Tutor tutor, IEnumerable<User> users, string onlineDocumentUrl, string name, decimal price)
        {
            if (users == null) throw new ArgumentNullException(nameof(users));
            if (price < 0) throw new ArgumentException(nameof(price));
            _users = users.Select(s => new StudyRoomUser(s, this)).ToList();
            Tutor = tutor;
            Identifier = ChatRoom.BuildChatRoomIdentifier(_users.Select(s => s.User.Id).Union(new[] { tutor.Id }));
            OnlineDocumentUrl = onlineDocumentUrl;
            Name = name;
            if (_users.Count < 10)
            {
                Type = StudyRoomType.PeerToPeer;
            }
            else
            {
                Type = StudyRoomType.GroupRoom;
            }

            DateTime = new DomainTimeStamp();
            Price = price;
            AddEvent(new StudyRoomCreatedEvent(this));
        }

        protected StudyRoom()
        {

        }

        public virtual string Name { get; set; }

        public virtual Tutor Tutor { get; protected set; }

        private readonly ICollection<StudyRoomUser> _users = new List<StudyRoomUser>();

        public virtual IEnumerable<StudyRoomUser> Users => _users;


        public virtual string Identifier { get; protected set; }
        public virtual DomainTimeStamp DateTime { get; protected set; }

        public virtual string OnlineDocumentUrl { get; protected set; }

        private readonly IList<StudyRoomSession> _sessions = new List<StudyRoomSession>();

        public virtual IEnumerable<StudyRoomSession> Sessions => _sessions;

        public virtual decimal? Price { get; protected set; }

        public virtual StudyRoomSession? GetCurrentSession()
        {
            var result = Sessions.AsQueryable().Where(w => w.Ended == null).OrderBy(o => o.Id).ToList();
            for (var i = 0; i < result.Count - 1; i++)
            {
                result[i].EndSession();
            }
            return result.SingleOrDefault(w => w.Ended == null);
        }

        public virtual StudyRoomType? Type { get; protected set; }

        public virtual void AddSession(string sessionName)
        {
            var session = new StudyRoomSession(this, sessionName);
            _sessions.Add(session);
            var user = Users.First(f => f.User.Id != Tutor.Id).User;
            user.UseToken(this);
            DateTime.UpdateTime = System.DateTime.UtcNow;
        }

        //public virtual void ChangeOnlineStatus(long userId, bool isOnline)
        //{
        //    var studyRoomUser = Users.Single(f => f.User.Id == userId);
        //    studyRoomUser.ChangeOnlineState(isOnline);
        //}

    }
}