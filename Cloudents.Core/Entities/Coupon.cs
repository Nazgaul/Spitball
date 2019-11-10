using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Cloudents.Core.Entities
{
    public enum CouponType
    {
        Flat,
        Percentage

    }

    public class Coupon
    {
        public Coupon([NotNull] string code, CouponType couponType, Tutor tutor, decimal value,
            int? amountOfUsers, int amountOfUsePerUser, DateTime? expiration, string description, string owner)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
            if (amountOfUsers.HasValue && amountOfUsers.Value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
            if (couponType == CouponType.Percentage && value > 100)
            {
                throw new ArgumentException("value cannot be more than 100");
            }


            Code = code ?? throw new ArgumentNullException(nameof(code));

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

        private readonly ISet<UserCoupon> _userCoupon = new HashSet<UserCoupon>();
        public virtual IEnumerable<UserCoupon> UserCoupon => _userCoupon;

        public virtual void ApplyCoupon(User user, Tutor tutor)
        {
            if (Expiration.GetValueOrDefault(DateTime.MaxValue) < DateTime.UtcNow)
            {
                throw new ArgumentException("invalid coupon");
            }

            if (AmountOfUsers.HasValue && AmountOfUsers.Value <= _userCoupon.Count)
            {
                throw new OverflowException();
            }
            var p = new UserCoupon(user, this, tutor);
            _userCoupon.Add(p);
        }


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