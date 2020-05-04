using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor" , Justification = "Nhibernate")]
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "Nhibernate")]
    public class UserCoupon : Entity<Guid>
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

        public virtual User User { get;protected set; }

        public virtual Coupon Coupon { get;protected set; }

        public virtual Tutor Tutor { get; protected set; }

        public virtual StudyRoomSessionUser? SessionUser { get;protected set; }

        public static readonly Expression<Func<UserCoupon, bool>> IsUsedExpression = x => x.UsedAmount < 1;

        //public static readonly Expression<Func<User, decimal>> CalculateBalanceExpression = x =>
        //    x.Transactions.Count();

        //private static readonly Func<User, decimal> CalculateBalance = CalculateBalanceExpression.Compile();

        //public virtual decimal Fee
        //{
        //    get { return CalculateBalance(this); }
        //}

        public virtual bool IsUsed()
        {
            return IsUsedExpression.Compile()(this);
            //return IsUsedExpression(this);
            //return UsedAmount < 1;
        }

        public virtual void UseCoupon(StudyRoomSessionUser user)
        {
            SessionUser = user;
            UsedAmount = 1;
        }

        public virtual int UsedAmount { get; protected set; }

        public virtual DateTime CreatedTime { get;protected set; }

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