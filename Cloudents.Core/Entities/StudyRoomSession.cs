using Cloudents.Core.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Nhibernate")]
    public class StudyRoomSession : Entity<Guid>
    {
        public const int StudyRoomNewVersion = 2;

        public StudyRoomSession(StudyRoom studyRoom, string sessionId)
        {
            StudyRoom = studyRoom;
            Created = DateTime.UtcNow;
            SessionId = sessionId;

            //UseUserToken();
            StudyRoomVersion = StudyRoomNewVersion;
            AddEvent(new StudyRoomSessionCreatedEvent(this));
        }
        protected StudyRoomSession()
        {

        }

        //protected virtual void UseUserToken()
        //{
        //    var user = StudyRoom.Users.First(f => f.User.Id != StudyRoom.Tutor.Id).User;
        //    user.UseToken(StudyRoom);

        //}

        public virtual int? StudyRoomVersion { get; set; }

        public virtual StudyRoom StudyRoom { get; protected set; }
        public virtual DateTime Created { get; protected set; }
        public virtual DateTime? Ended { get; protected set; }
        public virtual TimeSpan? Duration { get; protected set; }


        //public virtual int RejoinCount { get; protected set; }
        public virtual string SessionId { get; protected set; }
        public virtual string? Receipt { get; protected set; }
        public virtual decimal? Price { get; protected set; }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "nhibernate proxy")]
        public virtual byte[] Version { get; protected set; }


        private readonly ISet<StudyRoomSessionUser> _roomSessionUsers = new HashSet<StudyRoomSessionUser>();
        public virtual IEnumerable<StudyRoomSessionUser> RoomSessionUsers => _roomSessionUsers;

        public virtual void AddUser(User user)
        {
            if (user.Id == StudyRoom.Tutor.Id)
            {
                return;
            }
            var sessionUser = new StudyRoomSessionUser(this, user);
            _roomSessionUsers.Add(sessionUser);
        }

        public virtual void UserDisconnect(User user, TimeSpan durationInRoom)
        {
            if (user.Id == StudyRoom.Tutor.Id)
            {
                return;
            }
            
            var sessionUser = RoomSessionUsers.Single(s => s.User == user);
            sessionUser.Disconnect(durationInRoom);
        }

        //private readonly IList<SessionParticipantDisconnect> _participantDisconnections = new List<SessionParticipantDisconnect>();

        //public virtual IEnumerable<SessionParticipantDisconnect> ParticipantDisconnections => _participantDisconnections;

        //public virtual bool VideoExists { get; protected set; }


        //public virtual IPaymentProvider Payment { get; protected set; }
        public virtual DateTime? PaymentApproved { get; protected set; }
        public virtual TimeSpan? AdminDuration { get; protected set; }
        public virtual TimeSpan? RealDuration { get; protected set; }


        //public virtual void UpdateVideo()
        //{
        //    VideoExists = true;
        //}

        protected virtual void CalculatePriceAndDuration()
        {
            Duration = Ended - Created;
           // var tutorPrice = StudyRoom.Tutor.Price.SubsidizedPrice ??
           Price = ((decimal) Math.Floor(Duration.Value.TotalMinutes) / 60) * StudyRoom.Tutor.Price.GetPrice();
        }

        public virtual void EditDuration(int minutes)
        {
            if (Ended == null)
            {
                throw new ArgumentException();
            }
            Ended = Created.AddMinutes(minutes);
            CalculatePriceAndDuration();
        }

        public virtual void EndSession()
        {
            if (Ended.HasValue)
            {
                return;
            }
            Ended = DateTime.UtcNow;
            CalculatePriceAndDuration();
            AddEvent(new EndStudyRoomSessionEvent(this));
        }

        //public virtual void ReJoinStudyRoom()
        //{
        //    RejoinCount++;
        //    AddEvent(new StudyRoomSessionRejoinEvent(this));
        //}

        public virtual void SetReceipt(string receipt)
        {
            if (string.IsNullOrEmpty(receipt))
            {
                throw new ArgumentException();
            }
            Receipt = receipt;
        }

        public virtual void SetReceiptAndAdminDate(string receipt, int adminDuration)
        {
            if (string.IsNullOrEmpty(receipt))
            {
                throw new ArgumentException();
            }
            Receipt = receipt;
            PaymentApproved = DateTime.UtcNow;
            AdminDuration = new TimeSpan(0, adminDuration, 0);
            //AdminDuration = adminDuration;
            //StudentPay = studentPay;
        }

        public virtual void SetRealDuration(TimeSpan realDuration)
        {
            RealDuration = realDuration;
        }
    }
}
