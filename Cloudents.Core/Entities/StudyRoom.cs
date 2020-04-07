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
        public StudyRoom(Tutor tutor, IEnumerable<User> users, string onlineDocumentUrl,
            string name, decimal price, DateTime? broadcastTime) : this()
        {
            if (users == null) throw new ArgumentNullException(nameof(users));
            if (price < 0) throw new ArgumentException(nameof(price));
            _users = users.Select(s => new StudyRoomUser(s, this)).ToList();
            Tutor = tutor;
            if (_users.Any())
            {
                StudyRoomType = StudyRoomType.Private;
                Identifier = ChatRoom.BuildChatRoomIdentifier(
                    _users.Select(s => s.User.Id).Union(new[] { tutor.Id }));
            }
            else
            {
                StudyRoomType = StudyRoomType.Broadcast;
                Identifier =  Guid.NewGuid().ToString();
                ChatRoom = ChatRoom.FromStudyRoom(this);
                //var chatRoom = ChatRoom.FromStudyRoom(Identifier);
            }

            OnlineDocumentUrl = onlineDocumentUrl;
            Name = name;
            if (_users.Count < 10 && _users.Count > 0)
            {
                Type = StudyRoomTopologyType.PeerToPeer;
            }
            else
            {
                Type = StudyRoomTopologyType.GroupRoom;
            }

            DateTime = new DomainTimeStamp();
            Price = price;
            BroadcastTime = broadcastTime;
            AddEvent(new StudyRoomCreatedEvent(this));
        }

        protected StudyRoom()
        {
            ChatRooms ??= new List<ChatRoom>();
        }

        protected internal virtual ICollection<ChatRoom> ChatRooms { get; set; }

        public virtual ChatRoom ChatRoom
        {
            get => ChatRooms.SingleOrDefault();
            set
            {
                ChatRooms.Clear();
                ChatRooms.Add(value);
            }
        }


        public virtual StudyRoomType StudyRoomType { get; protected set; }

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

        public virtual DateTime? BroadcastTime { get; protected set; }

        public virtual StudyRoomSession? GetCurrentSession()
        {
            var result = Sessions.AsQueryable().Where(w => w.Ended == null).OrderBy(o => o.Id).ToList();
            for (var i = 0; i < result.Count - 1; i++)
            {
                result[i].EndSession();
            }
            return result.SingleOrDefault(w => w.Ended == null);
        }

        public virtual StudyRoomTopologyType? Type { get; protected set; }

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