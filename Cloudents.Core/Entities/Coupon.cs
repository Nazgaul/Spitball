using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]
    public class Coupon :Entity<Guid>
    {
        public const int MinimumLength = 5, MaxLength = 12;

        public Coupon(string code, CouponType couponType, decimal value,
            DateTime? expiration) : this(code, couponType, null, value, expiration)
        {
        //    if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
        //    if (couponType == CouponType.Percentage && value > 100)
        //    {
        //        throw new ArgumentException("value cannot be more than 100");
        //    }

        //    if (code is null)
        //    {
        //        throw new ArgumentNullException(nameof(code));
        //    }
        //    if (!code.Length.IsBetween(MinimumLength, MaxLength))
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(code));
        //    }

        //    Code = code;

        //    CouponType = couponType;
        //    Value = value;
        //    Expiration = expiration;
        //    CreateTime = DateTime.UtcNow;
        //    UserCoupon = new HashSet<UserCoupon>();

        }

        internal Coupon(string code, CouponType couponType, Tutor? tutor, decimal value,
            DateTime? expiration)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
            if (couponType == CouponType.Percentage && value > 100)
            {
                throw new ArgumentException("value cannot be more than 100");
            }

            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }
            if (!code.Length.IsBetween(MinimumLength, MaxLength))
            {
                throw new ArgumentOutOfRangeException(nameof(code));
            }

            Code = code;

            CouponType = couponType;
            Tutor = tutor;
            Value = value;
            Expiration = expiration;
            CreateTime = DateTime.UtcNow;
            UserCoupon = new HashSet<UserCoupon>();

        }

        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected Coupon()
        {

        }

     
        public virtual string Code { get; protected set; }

        public virtual CouponType CouponType { get; protected set; }

        public virtual Tutor? Tutor { get; protected set; }

        public virtual decimal Value { get; protected set; }


        public virtual DateTime? Expiration { get; protected set; }
        public virtual DateTime CreateTime { get; protected set; }

        public virtual string? Description { get; protected set; }

        protected internal virtual ISet<UserCoupon> UserCoupon { get;protected set; }
    
        


        public virtual bool CanApplyCoupon()
        {
            if (Expiration.GetValueOrDefault(DateTime.MaxValue) < DateTime.UtcNow)
            {
                throw new ArgumentException("invalid coupon");
            }

           

            return true;
        }

       

        public static double CalculatePrice(CouponType type, double price, decimal couponValue)
        {
            var d = (double) couponValue;
            var result = type switch
            {
                CouponType.Flat => (price - d),
                CouponType.Percentage => (price * ((100 - d) / 100)),
                _ => throw new ArgumentOutOfRangeException()
            };

            return Math.Max(result, 0);
        }


        protected bool Equals(Coupon other)
        {
            return Equals(Tutor.Id, other.Tutor.Id)
                   && Equals(Code, other.Code);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Coupon)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return HashCode.Combine(Tutor.Id.GetHashCode(), Code.GetHashCode(StringComparison.OrdinalIgnoreCase),
                    127);
            }
        }

    }
}