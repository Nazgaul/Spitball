using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Extension;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities
{
    public enum CouponType
    {
        Flat,
        Percentage

    }

    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]
    public class Coupon
    {
        public const int MinimumLength = 5, MaxLength = 12;
        public Coupon([NotNull] string code, CouponType couponType, Tutor tutor, decimal value,
            int? amountOfUsers, int amountOfUsePerUser, DateTime? expiration, string description, string owner)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
            if (amountOfUsers.HasValue && amountOfUsers.Value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
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
            AmountOfUsers = amountOfUsers;
            AmountOfUsePerUser = amountOfUsePerUser;
            Expiration = expiration;
            Description = description;
            Owner = owner;
            CreateTime = DateTime.UtcNow;

        }

        protected Coupon()
        {

        }

        public virtual Guid Id { get; protected set; }

        public virtual string Code { get; protected set; }

        public virtual CouponType CouponType { get; protected set; }

        public virtual Tutor Tutor { get; protected set; }

        public virtual decimal Value { get; protected set; }

        public virtual int? AmountOfUsers { get; protected set; }
        public virtual int AmountOfUsePerUser { get; protected set; }

        public virtual DateTime? Expiration { get; protected set; }
        public virtual DateTime? CreateTime { get; protected set; }

        public virtual string Description { get; protected set; }

        public virtual string Owner { get; protected set; }

        protected internal virtual ISet<UserCoupon> UserCoupon { get; set; }
    
        


        public virtual bool CanApplyCoupon()
        {
            if (Expiration.GetValueOrDefault(DateTime.MaxValue) < DateTime.UtcNow)
            {
                throw new ArgumentException("invalid coupon");
            }

            if (AmountOfUsers.HasValue && AmountOfUsers.Value <= UserCoupon.Count)
            {
                throw new OverflowException();
            }

            return true;
        }

        //public virtual void ApplyCoupon(User user, Tutor tutor)
        //{
        //    if (Expiration.GetValueOrDefault(DateTime.MaxValue) < DateTime.UtcNow)
        //    {
        //        throw new ArgumentException("invalid coupon");
        //    }

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
            decimal result;
            switch (type)
            {
                case CouponType.Flat:
                    result = price - couponValue;
                    break;
                case CouponType.Percentage:
                    result = price * ((100 - couponValue) / 100);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Math.Max(result, 0);
        }

    }
}