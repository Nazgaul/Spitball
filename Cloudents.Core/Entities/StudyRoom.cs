﻿using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Cloudents.Persistence")]
namespace Cloudents.Core.Entities
{
    public class PrivateStudyRoom : StudyRoom
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public PrivateStudyRoom(Tutor tutor, IEnumerable<User> users, string onlineDocumentUrl,
            string name, decimal price) : base(tutor,users,  price)
        {
            if (_users.Count == 0)
            {
                throw new ArgumentException();
            }

            Name = name;
            Identifier = ChatRoom.BuildChatRoomIdentifier(
                _users.Select(s => s.User.Id).Union(new[] { tutor.Id }));
            OnlineDocumentUrl = onlineDocumentUrl;
            if (_users.Count < 4 && _users.Count > 0)
            {
                TopologyType = StudyRoomTopologyType.SmallGroup;
            }
            else
            {
                TopologyType = StudyRoomTopologyType.GroupRoom;
            }
        }
        protected PrivateStudyRoom(): base()
        {
        }

        public virtual string Name { get; set; }

        public override void AddUserToStudyRoom(User user)
        {
            if (Tutor.User.Id == user.Id)
            {
                return;
            }
            var _ = Users.AsQueryable().Single(s => s.User.Id == user.Id);
        }

        public virtual void AddPayment(User user,string receipt)
        {
            var studyRoomPayment = new StudyRoomPayment(this, user, receipt);
            _studyRoomPayments.Add(studyRoomPayment);
        }

        public override StudyRoomType Type => StudyRoomType.Private;
    }

    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public abstract class StudyRoom : Entity<Guid>, IAggregateRoot
    {
        public StudyRoom(Tutor tutor, IEnumerable<User> users, 
             decimal price) : this()
        {
            if (users == null) throw new ArgumentNullException(nameof(users));
            if (price < 0) throw new ArgumentException(nameof(price));
            _users = new HashSet<StudyRoomUser>(users.Select(s => new StudyRoomUser(s, this)));
            Tutor = tutor;
            //OnlineDocumentUrl = onlineDocumentUrl;
            //Name = name;
          

            DateTime = new DomainTimeStamp();
            OldPrice = price;
            Price = new Money(price,Tutor.User.SbCountry.RegionInfo.ISOCurrencySymbol);
           
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

        

        public virtual Tutor Tutor { get; protected set; }

        protected readonly ISet<StudyRoomUser> _users = new HashSet<StudyRoomUser>();

        public virtual IEnumerable<StudyRoomUser> Users => _users;

        protected readonly ICollection<StudyRoomPayment> _studyRoomPayments = new List<StudyRoomPayment>();

        public virtual IEnumerable<StudyRoomPayment> StudyRoomPayments => _studyRoomPayments;
        public virtual string Identifier { get; protected set; }
        public virtual DomainTimeStamp DateTime { get; protected set; }

        public virtual string? OnlineDocumentUrl { get; set; }

        private readonly IList<StudyRoomSession> _sessions = new List<StudyRoomSession>();

        public virtual IEnumerable<StudyRoomSession> Sessions => _sessions;

        [Obsolete]
        public virtual decimal OldPrice { get; protected set; }


        public virtual Money Price { get; protected set; }
        public virtual void SetPrice(decimal price)
        {
            OldPrice = price;
            Price = new Money(price,Tutor.User.SbCountry.RegionInfo.ISOCurrencySymbol);
        }

        public virtual StudyRoomSession? GetCurrentSession()
        {
            var result = Sessions.AsQueryable().Where(w => w.Ended == null).OrderBy(o => o.Id).ToList();
            for (var i = 0; i < result.Count - 1; i++)
            {
                result[i].EndSession();
            }
            return result.SingleOrDefault(w => w.Ended == null);
        }

        public virtual StudyRoomTopologyType TopologyType { get; protected set; }

        public virtual void ChangeTopologyDbi()
        {
            if (_users.Count() < 4 && _users.Count() > 0)
            {
                TopologyType = StudyRoomTopologyType.SmallGroup;
            }
            else
            {
                TopologyType = StudyRoomTopologyType.GroupRoom;
            }
        }

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