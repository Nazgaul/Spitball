using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;

namespace Cloudents.Core.Entities
{
    public class StudyRoomSessionUser : Entity<Guid>, IEquatable<StudyRoomSessionUser>
    {
        public StudyRoomSessionUser(StudyRoomSession studyRoomSession, User user)
        {
            StudyRoomSession = studyRoomSession;
            User = user;
            if (studyRoomSession.StudyRoom.Price.HasValue)
            {
                PricePerHour = studyRoomSession.StudyRoom.Price.Value;
            }
            else
            {
                PricePerHour = studyRoomSession.StudyRoom.Tutor.Price.GetPrice();
            }

            UseCoupon();
            UsePaymentToken();
        }
        [SuppressMessage("ReSharper", "CS8618", Justification = "Nhibernate proxy")]
        protected StudyRoomSessionUser()
        {
        }



        public virtual StudyRoomSession StudyRoomSession { get; protected set; }

        public virtual User User { get; protected set; }

        public virtual TimeSpan? Duration { get; protected set; }

        public virtual int DisconnectCount { get; protected set; }

        public virtual decimal PricePerHour { get; protected set; }

        public virtual TimeSpan? TutorApproveTime { get; protected set; }

        public virtual decimal TotalPrice { get; protected set; }

        public virtual string? Receipt { get; protected set; }

        public virtual void Disconnect(TimeSpan durationInRoom)
        {
            Duration = Duration.GetValueOrDefault(TimeSpan.Zero) + durationInRoom;
            TotalPrice = (decimal)Duration.Value.TotalHours * PricePerHour;
            DisconnectCount++;
        }

        public virtual void ApproveSession(TimeSpan duration, decimal price)
        {
            PricePerHour = price;
            ApproveSession(duration);
        }

        protected virtual void ApproveSession(TimeSpan duration)
        {
            TutorApproveTime = duration;
            TotalPrice = (decimal)TutorApproveTime.Value.TotalHours * PricePerHour;
        }

        public virtual void NoPay()
        {
            Receipt = "No Pay";
            TutorApproveTime = Duration;
            TotalPrice = 0;
        }

        public virtual void Pay(in string receipt, in TimeSpan duration, in decimal price)
        {
            Receipt = receipt;
            ApproveSession(duration);
            TotalPrice = price;

            //Use Copuon
        }

        public virtual void UsePaymentToken()
        {
            if (User.SbCountry != Entities.Country.UnitedStates)
            {
                return;
            }

            if (PricePerHour == 0)
            {
                return;
            }
            var userToken = this.StudyRoomSession.StudyRoom.UserTokens.FirstOrDefault(w => w.State == PaymentTokenState.NotUsed);
            if (userToken != null)
            {
                userToken.ChangeToUsedState(this);
            }
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

        //public static bool operator ==(StudyRoomSessionUser left, StudyRoomSessionUser right)
        //{
        //    return Equals(left, right);
        //}

        //public static bool operator !=(StudyRoomSessionUser left, StudyRoomSessionUser right)
        //{
        //    return !Equals(left, right);
        //}

     
    }
}
