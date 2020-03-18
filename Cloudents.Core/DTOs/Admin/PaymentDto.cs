﻿using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class PaymentDto
    {
        [EntityBind(nameof(StudyRoomSession.Id))]
        public Guid StudyRoomSessionId { get; set; }
        [EntityBind(nameof(Tutor.Price))]
        public decimal Price { get; set; }

        [EntityBind(nameof(Tutor.SellerKey))]
        public bool IsSellerKeyExists { get; set; }
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
        public TimeSpan Duration { get; set; }
        public TimeSpan RealDuration { get; set; }
        [EntityBind(nameof(User.PaymentExists))]
        public bool IsPaymentKeyExists { get; set; }
        public bool IsRealDurationExitsts { get; set; }
}

    public class PaymentDetailDto
    {
        [EntityBind(nameof(StudyRoomSession.Id))]
        public Guid StudyRoomSessionId { get; set; }
        [EntityBind(nameof(Tutor.Price))]
        public decimal TutorPricePerHour { get; set; }

        
        public decimal StudentPayPerHour { get; set; }
        public decimal SpitballPayPerHour { get; set; }


        [EntityBind(nameof(Tutor.SellerKey))]
        public bool CantPay { get; set; }
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
        public long DurationInTicks { get; set; }
        public bool ShouldSerializeDurationInTicks() => false;
        public double Duration => TimeSpan.FromTicks(DurationInTicks).TotalMinutes;

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
