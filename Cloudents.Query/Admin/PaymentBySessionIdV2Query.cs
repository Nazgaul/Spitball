﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Linq;

namespace Cloudents.Query.Admin
{
    public class PaymentBySessionIdV2Query : IQuery<PaymentDetailDto?>
    {
        public PaymentBySessionIdV2Query(Guid sessionId, long userId, long tutorId)
        {
            SessionId = sessionId;
            TutorId = tutorId;
            UserId = userId;
        }

        private Guid SessionId { get; }
        private long UserId { get; }

        private long TutorId { get; }

        internal sealed class PaymentBySessionIdQueryHandler : IQueryHandler<PaymentBySessionIdV2Query, PaymentDetailDto?>
        {
            private readonly IStatelessSession _stateless;

            public PaymentBySessionIdQueryHandler(QuerySession querySession)
            {
                _stateless = querySession.StatelessSession;
            }

            public async Task<PaymentDetailDto?> GetAsync(PaymentBySessionIdV2Query query, CancellationToken token)
            {



                var couponFuture = _stateless.Query<UserCoupon>()
                    .Fetch(f => f.Coupon)
                    .Where(w => w.StudyRoomSessionUser!.StudyRoomSession.Id == query.SessionId)
                    .Select(s => new CouponClass
                    {
                        Value = s.Coupon.Value,
                        TutorId = s.Coupon.Tutor!.Id,
                        CouponType = s.Coupon.CouponType,
                        Code = s.Coupon.Code
                        
                    })
                    .Take(1)
                    .ToFutureValue();


                var paymentFuture = _stateless.Query<StudyRoomPayment>()
                    .Fetch(f=>f.StudyRoomSessionUser)
                   // .Fetch(f => f.StudyRoomSession)
                    //.ThenFetch(f => f.StudyRoom)
                    .Fetch(f => f.Tutor)
                    .ThenFetch(f => f.User)
                    .Fetch(f => f.User)
                    .Where(w => w.User.Id == query.UserId)
                    .Where(w => w.Id == query.SessionId)
                    .Select(s => new PaymentDetailDto
                    {
                        TutorPricePerHour = s.PricePerHour.Amount,
                        UserId = s.User.Id,
                        TutorId = query.TutorId,
                        UserName = s.User.Name,
                        Created = s.Created,
                        _sellerKey = s.Tutor.SellerKey,
                        TutorCountry = s.Tutor.User.SbCountry,
                        _duration = s.TutorApproveTime ?? s.StudyRoomSessionUser!.Duration,
                        TutorName = s.Tutor.User.Name,
                        StudyRoomSessionId = s.Id

                    }).ToFutureValue();

                var couponResult = await couponFuture.GetValueAsync(token);
                var payment = paymentFuture.Value;

                if (couponResult == null)
                {
                    return payment;
                }

                if (couponResult.TutorId != 0 && couponResult.TutorId != query.TutorId)
                {
                    return payment;
                }

                payment.StudentPayPerHour = Coupon.CalculatePrice(couponResult.CouponType, payment.TutorPricePerHour, couponResult.Value);

                if (couponResult.TutorId == 0)
                {
                    payment.SpitballPayPerHour = payment.TutorPricePerHour - payment.StudentPayPerHour;
                }

                payment.CouponCode = couponResult.Code;
                payment.CouponType = couponResult.CouponType;
                payment.CouponValue = couponResult.Value;
                payment.CouponTutor = couponResult.TutorId;
                return payment;

            }

            private class CouponClass
            {
                public string Code { get; set; }
                public CouponType CouponType { get; set; }
                public decimal Value { get; set; }
                public long? TutorId { get; set; }

              


            }
        }

    }

    
}