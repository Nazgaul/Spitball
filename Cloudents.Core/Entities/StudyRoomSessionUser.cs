using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Entities
{
    public class StudyRoomSessionUser : Entity<Guid>, IEquatable<StudyRoomSessionUser>
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public StudyRoomSessionUser(StudyRoomSession studyRoomSession, User user , StudyRoomPayment? studyRoomPayment)
        {
            StudyRoomSession = studyRoomSession;
            User = user;
            if (studyRoomPayment == null)
            {
                StudyRoomPayment = new StudyRoomPayment(this);
            }
            else
            {
                studyRoomPayment.StudyRoomSessionUser =  this;
                StudyRoomPayment = studyRoomPayment;


            }
            //UseCoupon();
        }
        [SuppressMessage("ReSharper", "CS8618", Justification = "Nhibernate proxy")]
        protected StudyRoomSessionUser()
        {
        }

        public virtual StudyRoomSession StudyRoomSession { get; }

        public virtual User User { get; protected set; }

        public virtual TimeSpan? Duration { get; protected set; }

        public virtual int DisconnectCount { get; protected set; }

     
        protected internal virtual ICollection<UserCoupon> UserCoupons { get;  set; }

        public virtual void Disconnect(TimeSpan durationInRoom)
        {
            Duration = Duration.GetValueOrDefault(TimeSpan.Zero) + durationInRoom;
            if (StudyRoomPayment != null)
            {
                StudyRoomPayment.TotalPrice = Duration.Value.TotalHours * StudyRoomPayment.PricePerHour;
            }

            DisconnectCount++;
        }

        //protected virtual void UseCoupon()
        //{
        //    var tutor = StudyRoomSession.StudyRoom.Tutor;
        //    var userCoupon = User.UserCoupon.SingleOrDefault(w => w.Tutor.Id == tutor.Id 
        //                                                     && w.IsNotUsed());
        //    if (userCoupon is null) // we do not check before if user have coupon on that user
        //    {
        //        return;
        //    }
        //    userCoupon.UseCoupon(this);
        //    AddEvent(new UseCouponEvent(userCoupon));
        //}


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

        public virtual StudyRoomPayment? StudyRoomPayment { get; set; }

    }
}
