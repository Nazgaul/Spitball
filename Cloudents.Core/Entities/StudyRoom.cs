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
    public class BroadCastStudyRoom : StudyRoom
    {
        public BroadCastStudyRoom(Tutor tutor,
            IEnumerable<User> users, string onlineDocumentUrl, string name, decimal price, DateTime broadcastTime) : base(tutor, users, onlineDocumentUrl, name, price)
        {
            Identifier = Guid.NewGuid().ToString();
            ChatRoom = ChatRoom.FromStudyRoom(this);
            BroadcastTime = broadcastTime;
            TopologyType = StudyRoomTopologyType.GroupRoom;
        }

        protected BroadCastStudyRoom(): base()
        {
        }

        public virtual DateTime BroadcastTime { get; protected set; }

        public override void AddUserToStudyRoom(User user)
        {
           
                var studyRoomUser = new StudyRoomUser(user, this);
                _users.Add(studyRoomUser);
                ChatRoom.AddUserToChat(user);
                Tutor.User.AddFollower(user);
            
        }

        public override StudyRoomType Type => StudyRoomType.Broadcast;
    }

    public class PrivateStudyRoom : StudyRoom
    {
        public PrivateStudyRoom(Tutor tutor, IEnumerable<User> users, string onlineDocumentUrl,
            string name, decimal price) : base(tutor,users,onlineDocumentUrl, name, price)
        {
            if (_users.Count == 0)
            {
                throw new ArgumentException();
            }
            Identifier = ChatRoom.BuildChatRoomIdentifier(
                _users.Select(s => s.User.Id).Union(new[] { tutor.Id }));
            if (_users.Count < 10 && _users.Count > 0)
            {
                TopologyType = StudyRoomTopologyType.PeerToPeer;
            }
            else
            {
                TopologyType = StudyRoomTopologyType.GroupRoom;
            }
        }
        protected PrivateStudyRoom(): base()
        {
        }

        public override void AddUserToStudyRoom(User user)
        {
            if (Tutor.User.Id == user.Id)
            {
                return;
            }
            var _ = Users.AsQueryable().Single(s => s.User.Id == user.Id);
        }

        public override StudyRoomType Type => StudyRoomType.Private;
    }

    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public abstract class StudyRoom : Entity<Guid>, IAggregateRoot
    {
        public StudyRoom(Tutor tutor, IEnumerable<User> users, string onlineDocumentUrl,
            string name, decimal price) : this()
        {
            if (users == null) throw new ArgumentNullException(nameof(users));
            if (price < 0) throw new ArgumentException(nameof(price));
            _users = new HashSet<StudyRoomUser>(users.Select(s => new StudyRoomUser(s, this)));
            Tutor = tutor;
           // StudyRoomType = type;
            //if (StudyRoomType == StudyRoomType.Private )
            //{
            //    if (_users.Count == 0)
            //    {
            //        throw new ArgumentException();
            //    }
            //    //StudyRoomType = StudyRoomType.Private;
            //    Identifier = ChatRoom.BuildChatRoomIdentifier(
            //        _users.Select(s => s.User.Id).Union(new[] { tutor.Id }));
            //}
            //else
            //{
            //    if (!broadcastTime.HasValue)
            //    {
            //        throw new ArgumentException();
            //    }
            //    StudyRoomType = StudyRoomType.Broadcast;
            //    Identifier = Guid.NewGuid().ToString();
            //    ChatRoom = ChatRoom.FromStudyRoom(this);
            //    BroadcastTime = broadcastTime!.Value;
            //    //var chatRoom = ChatRoom.FromStudyRoom(Identifier);
            //}

            OnlineDocumentUrl = onlineDocumentUrl;
            Name = name;
            //if (_users.Count < 10 && _users.Count > 0)
            //{
            //    Type = StudyRoomTopologyType.PeerToPeer;
            //}
            //else
            //{
            //    Type = StudyRoomTopologyType.GroupRoom;
            //}

            DateTime = new DomainTimeStamp();
            Price = price;
           
            AddEvent(new StudyRoomCreatedEvent(this));
        }

        [SuppressMessage("ReSharper", "CS8618",Justification = "Nhibernate proxy")]
        protected StudyRoom()
        {
            ChatRooms ??= new List<ChatRoom>();
        }

        protected internal virtual ICollection<ChatRoom> ChatRooms { get; set; }

        protected virtual ChatRoom ChatRoom
        {
            get => ChatRooms.SingleOrDefault();
            set
            {
                ChatRooms.Clear();
                ChatRooms.Add(value);
            }
        }


      //  public virtual StudyRoomType StudyRoomType { get; protected set; }

        public virtual string Name { get; set; }

        public virtual Tutor Tutor { get; protected set; }

        protected readonly ISet<StudyRoomUser> _users = new HashSet<StudyRoomUser>();

        public virtual IEnumerable<StudyRoomUser> Users => _users;


        public virtual string Identifier { get; protected set; }
        public virtual DomainTimeStamp DateTime { get; protected set; }

        public virtual string OnlineDocumentUrl { get; protected set; }

        private readonly IList<StudyRoomSession> _sessions = new List<StudyRoomSession>();

        public virtual IEnumerable<StudyRoomSession> Sessions => _sessions;

        public virtual decimal? Price { get; protected set; }

        

        private readonly ICollection<UserPaymentToken> _userTokens = new List<UserPaymentToken>();
        public virtual IEnumerable<UserPaymentToken> UserTokens => _userTokens;

        public virtual StudyRoomSession? GetCurrentSession()
        {
            var result = Sessions.AsQueryable().Where(w => w.Ended == null).OrderBy(o => o.Id).ToList();
            for (var i = 0; i < result.Count - 1; i++)
            {
                result[i].EndSession();
            }
            return result.SingleOrDefault(w => w.Ended == null);
        }

        public virtual StudyRoomTopologyType? TopologyType { get; protected set; }

        public virtual void AddSession(string sessionName)
        {
            var session = new StudyRoomSession(this, sessionName);
            _sessions.Add(session);
          
           
            DateTime.UpdateTime = System.DateTime.UtcNow;
        }

        public abstract void AddUserToStudyRoom(User user);
        public abstract StudyRoomType Type { get; }


        

      

    }
}