using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Extension;

namespace Cloudents.Core.Entities
{
    public enum CouponType
    {
        Flat,
        Percentage

    }

    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]
    public class Coupon :Entity<Guid>
    {
        public const int MinimumLength = 5, MaxLength = 12;
        public Coupon(string code, CouponType couponType,
            Course course, double value,
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
            Course = course ?? throw new ArgumentNullException(nameof(course));
           // Tutor = course.Tutor;
            Value = value;
            Expiration = expiration;
            CreateTime = DateTime.UtcNow;
            UserCoupon = new HashSet<UserCoupon>();

        }

        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected Coupon()
        {

        }


        public virtual string Code { get;  }

        public virtual CouponType CouponType { get;  }
        public virtual Course Course { get; }

        public virtual Tutor? Tutor { get; protected set; }
        public virtual double Value { get; }

        [Obsolete] public virtual decimal ValueOld{ get; set; }
        


        public virtual DateTime? Expiration { get; protected set; }
        public virtual DateTime? CreateTime { get; protected set; }

        public virtual string? Description { get; protected set; }

        protected internal virtual ISet<UserCoupon> UserCoupon { get;protected set; }
    
        


        public virtual bool CanApplyCoupon()
        {
            if (Expiration.GetValueOrDefault(DateTime.MaxValue) < DateTime.UtcNow)
            {
                throw new ArgumentException("invalid coupon");
            }

            //if (AmountOfUsers.HasValue && AmountOfUsers.Value <= UserCoupon.Count)
            //{
            //    throw new OverflowException();
            //}

            return true;
        }

      

        //public static double CalculatePrice(CouponType type, double price, decimal couponValue)
        //{
        //    var d = (double) couponValue;
        //    var result = type switch
        //    {
        //        CouponType.Flat => (price - d),
        //        CouponType.Percentage => (price * ((100 - d) / 100)),
        //        _ => throw new ArgumentOutOfRangeException()
        //    };

        //    if (AmountOfUsers.HasValue && AmountOfUsers.Value <= _userCoupon.Count)
        //    {
        //        throw new OverflowException();
        //    }
        //    var p = new UserCoupon(user, this, tutor);
        //    if (!_userCoupon.Add(p))
        //    {
        //        throw new ArgumentException("user already applied coupon");
        //    }
        //}


        public static decimal CalculatePrice(CouponType type, decimal price, decimal couponValue)
        {
            var result = type switch
            {
                CouponType.Flat => (price - couponValue),
                CouponType.Percentage => (price * ((100 - couponValue) / 100)),
                _ => throw new ArgumentOutOfRangeException()
            };

            return Math.Max(result, 0);
        }

        public static double CalculatePrice(CouponType type, double price, double couponValue)
        {
            var d =  couponValue;
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
            return Course.Id.Equals(other.Course.Id) && Code.Equals(other.Code);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Coupon) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(41, Course.Id, Code);
        }

    }
}