﻿using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class PaymentDto
    {
        [NonSerialized]
        public TimeSpan? _duration;
        [NonSerialized]
        public TimeSpan? _realDuration;
        [NonSerialized]
        public string? _sellerKey;

        [EntityBind(nameof(StudyRoomSession.Id))]
        public Guid StudyRoomSessionId { get; set; } //
        [EntityBind(nameof(Tutor.Price))]
        public decimal Price { get; set; } //

        [EntityBind(nameof(Tutor.SellerKey))]
        public bool IsSellerKeyExists
        {
            get
            {
                if (TutorCountry == Country.UnitedStates)
                {
                    return true;
                }

                return _sellerKey != null;
            }
        } //
        [EntityBind(nameof(Tutor.Id))]
        public long TutorId { get; set; } //
        [EntityBind(nameof(User.Name))]
        public string TutorName { get; set; } //
        [EntityBind(nameof(User.Id))]
        public long UserId { get; set; } //
        [EntityBind(nameof(User.Name))]
        public string UserName { get; set; } //
        [EntityBind(nameof(StudyRoomSession.Created))]
        public DateTime Created { get; set; } //

        public Country TutorCountry { get; set; }

        public double Duration => _duration.GetValueOrDefault().TotalMinutes;

        public double? RealDuration => _realDuration?.TotalMinutes;


        [EntityBind(nameof(User.PaymentExists))]
        public bool IsPaymentKeyExists { get; set; } //

        public bool IsRealDurationExists => RealDuration.HasValue; //
    }

    public class PaymentDetailDto
    {
        [NonSerialized]
        public TimeSpan? _duration;
        [NonSerialized]
        public string? _sellerKey;

        [NonSerialized] public long? _duration2;

        [EntityBind(nameof(StudyRoomSession.Id))]
        public Guid StudyRoomSessionId { get; set; }
        [EntityBind(nameof(Tutor.Price))]
        public decimal TutorPricePerHour { get; set; }

        public Country TutorCountry { get; set; }

        public bool CantPay
        {
            get
            {
                if (TutorCountry == Country.UnitedStates)
                {
                    return false;
                }

                return _sellerKey == null;
            }
        } //


        public decimal StudentPayPerHour { get; set; }
        public decimal SpitballPayPerHour { get; set; }

        [EntityBind(nameof(Tutor.Id))]
        public long TutorId { get; set; }
        [EntityBind(nameof(User.Name))]
        public string TutorName { get; set; }
        [EntityBind(nameof(User.Id))]
        public long UserId { get; set; }
        [EntityBind(nameof(User.Name))]
        public string UserName { get; set; }
        [EntityBind(nameof(StudyRoomSession.Created))]
        public DateTime Created { get; set; }
        //public TimeSpan Duration { get; set; }
        //public bool ShouldSerializeDurationInTicks() => false;
        public double Duration
        {

            get
            {
                if (_duration.HasValue)
                {
                    return _duration.Value.TotalMinutes;
                }

                return TimeSpan.FromTicks(_duration2.Value).TotalMinutes;
            }
        }

        [EntityBind(nameof(Coupon.Code))]
        public string CouponCode { get; set; }
        [EntityBind(nameof(Coupon.CouponType))]
        public CouponType? CouponType { get; set; }
        [EntityBind(nameof(Coupon.Value))]
        public decimal? CouponValue { get; set; }

        [EntityBind(nameof(Coupon.Tutor.Id))]
        public long? CouponTutor { get; set; }

    }
}
