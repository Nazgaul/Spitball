using System;

namespace Cloudents.Core.Entities
{
    public class UserCoupon
    {
        public UserCoupon(User user, Coupon coupon, Tutor tutor)
        {
            if (user.Id == tutor.Id)
            {
                throw new ArgumentException();
            }
            User = user;
            Coupon = coupon;
            Tutor = tutor;
            CreatedTime = DateTime.UtcNow;
        }

        protected UserCoupon()
        {

        }

        public virtual Guid Id { get; set; }
        public virtual User User { get; set; }

        public virtual Coupon Coupon { get; set; }

        public virtual Tutor Tutor { get; set; }

        public virtual int UsedAmount { get; set; }

        public virtual DateTime CreatedTime { get; set; }

        protected bool Equals(UserCoupon other)
        {
            return Equals(User.Id, other.User.Id) 
                   && Equals(Tutor.Id, other.Tutor.Id) 
                   && Equals(Coupon.Id,other.Coupon.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserCoupon)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
               return  ((User != null ? User.Id.GetHashCode() : 0) * 397) ^
                       (Tutor != null ? Tutor.Id.GetHashCode() : 0) ^
                       ((Coupon != null ? Coupon.Id.GetHashCode() : 0) * 11);
            }
        }

    }
}