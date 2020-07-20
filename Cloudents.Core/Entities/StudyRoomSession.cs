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

        public static readonly TimeSpan BillableStudyRoomSession = TimeSpan.FromMinutes(10);

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


        public virtual long? DurationTicks { get; protected set; }


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

            var studyRoomPayment = this.StudyRoom.StudyRoomPayments.SingleOrDefault(w => w.User.Id == user.Id);

            var sessionUser = new StudyRoomSessionUser(this, user, studyRoomPayment);
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

        [Obsolete]
        public virtual DateTime? PaymentApproved { get; protected set; }
        [Obsolete]
        public virtual TimeSpan? AdminDuration { get; protected set; }
        [Obsolete]
        public virtual TimeSpan? RealDuration { get; protected set; }



        [Obsolete]
        protected virtual void CalculatePriceAndDuration()
        {
            Duration = Ended - Created;
            // var tutorPrice = StudyRoom.Tutor.Price.SubsidizedPrice ??
            Price = (decimal)(Math.Floor(Duration.Value.TotalMinutes) / 60 * StudyRoom.Price.Amount);
        }

        [Obsolete]
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
            Duration = Ended - Created;
            AddEvent(new EndStudyRoomSessionEvent(this));
        }


        [Obsolete]
        public virtual void SetReceipt(string receipt)
        {
            if (string.IsNullOrEmpty(receipt))
            {
                throw new ArgumentException();
            }
            Receipt = receipt;
        }

        [Obsolete]
        public virtual void SetReceiptAndAdminDate(string receipt, TimeSpan adminDuration)
        {
            if (string.IsNullOrEmpty(receipt))
            {
                throw new ArgumentException();
            }
            Receipt = receipt;
            PaymentApproved = DateTime.UtcNow;
            AdminDuration = adminDuration;
        }

        [Obsolete]
        public virtual void SetRealDuration(TimeSpan realDuration, double price)
        {
            Price = (decimal)price;
            RealDuration = realDuration;
        }
    }
}
