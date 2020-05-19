using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.Event;

namespace Cloudents.Core.Entities
{
    public class StudyRoomSessionUser : Entity<Guid>, IEquatable<StudyRoomSessionUser>
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public StudyRoomSessionUser(StudyRoomSession studyRoomSession, User user)
        {
            StudyRoomSession = studyRoomSession;
            User = user;
            StudyRoomPayment = new StudyRoomPayment(this);
            UseCoupon();
        }
        [SuppressMessage("ReSharper", "CS8618", Justification = "Nhibernate proxy")]
        protected StudyRoomSessionUser()
        {
        }

        public virtual StudyRoomSession StudyRoomSession { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual TimeSpan? Duration { get; protected set; }

        public virtual int DisconnectCount { get; protected set; }

        [Obsolete]
        public virtual decimal PricePerHour { get; protected set; }

        [Obsolete]
        public virtual TimeSpan? TutorApproveTime { get; protected set; }

        [Obsolete]
        public virtual decimal TotalPrice { get; protected set; }

        [Obsolete]
        public virtual string? Receipt { get; protected set; }
     

        public virtual void Disconnect(TimeSpan durationInRoom)
        {
            Duration = Duration.GetValueOrDefault(TimeSpan.Zero) + durationInRoom;
            StudyRoomPayment.TotalPrice = Duration.Value.TotalHours * StudyRoomPayment.PricePerHour;
            DisconnectCount++;
        }

        protected virtual void UseCoupon()
        {
            var tutor = StudyRoomSession.StudyRoom.Tutor;
            var userCoupon = User.UserCoupon.SingleOrDefault(w => w.Tutor.Id == tutor.Id 
                                                             && w.IsNotUsed());
            if (userCoupon is null) // we do not check before if user have coupon on that user
            {
                return;
            }
            userCoupon.UseCoupon(this);
            //'userCoupon.UsedAmount++;
            AddEvent(new UseCouponEvent(userCoupon));
        }


        public virtual bool Equals( StudyRoomSessionUser other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StudyRoomSession.Id.Equals(other.StudyRoomSession.Id) && User.Id.Equals(other.User.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StudyRoomSessionUser)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = StudyRoomSession.Id.GetHashCode();
                hashCode = (hashCode * 397) ^ User.Id.GetHashCode();
                return hashCode;
            }
        }

        public virtual StudyRoomPayment StudyRoomPayment { get; set; }

    }

    public class StudyRoomPayment : Entity<Guid>
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public StudyRoomPayment(StudyRoomSessionUser studyRoomSessionUser)
        {
            PricePerHour = (double) (studyRoomSessionUser.StudyRoomSession.StudyRoom.Price ??
                                     studyRoomSessionUser.StudyRoomSession.StudyRoom.Tutor.Price.GetPrice());
            Tutor = studyRoomSessionUser.StudyRoomSession.StudyRoom.Tutor;
            User = studyRoomSessionUser.User;
            StudyRoomSessionUser = studyRoomSessionUser;
            Created = DateTime.UtcNow;;
        }

        [SuppressMessage("ReSharper", "CS8618", Justification = "Nhibernate proxy")]
        protected StudyRoomPayment()
        {
        }

        public virtual void ApproveSession(TimeSpan duration, double price)
        {
            PricePerHour = price;
            ApproveSession(duration);
        }

        protected virtual void ApproveSession(TimeSpan duration)
        {
            TutorApproveTime = duration;
            TotalPrice = TutorApproveTime.Value.TotalHours * PricePerHour;
        }

        public virtual void NoPay()
        {
            Receipt = "No Pay";
            TutorApproveTime ??= StudyRoomSessionUser?.Duration;
            TotalPrice = 0;
        }

        public virtual void Pay(string receipt, TimeSpan duration,  double price)
        {
            Receipt = receipt;
            ApproveSession(duration);
            TotalPrice = price;
        }

        public virtual StudyRoomSessionUser? StudyRoomSessionUser { get; protected set; }

        public virtual TimeSpan? TutorApproveTime { get; protected set; }

        public virtual double PricePerHour { get; protected set; }

        public virtual double TotalPrice { get;  set; }

        public virtual string? Receipt { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual Tutor Tutor { get;protected set; }

        public virtual DateTime Created { get;protected set; }
    }
}
