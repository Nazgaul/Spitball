﻿using System;
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
    public class Coupon
    {
        public const int MinimumLength = 5, MaxLength = 12;
        public Coupon(string code, CouponType couponType, Tutor? tutor, decimal value,
            DateTime? expiration, string? description)
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
            Description = description;
            CreateTime = DateTime.UtcNow;
            UserCoupon = new HashSet<UserCoupon>();

        }

        [SuppressMessage("ReSharper", "CS8618", Justification = "nhibernate")]
        protected Coupon()
        {

        }

        public virtual Guid Id { get; protected set; }

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

        //public static decimal CalculatePrice(CouponType type, decimal price, decimal couponValue)
        //{
        //    var result = type switch
        //    {
        //        CouponType.Flat => (price - couponValue),
        //        CouponType.Percentage => (price * ((100 - couponValue) / 100)),
        //        _ => throw new ArgumentOutOfRangeException()
        //    };

        //    return Math.Max(result, 0);
        //}

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

    }
}