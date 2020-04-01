using System;
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
                    .Where(w => w.User.Id == query.UserId && w.Tutor.Id == query.TutorId)
                    .Where(w => w.UsedAmount < w.Coupon.AmountOfUsePerUser)
                    .Select(s => new CouponClass
                    {
                        Value = s.Coupon.Value,
                        TutorId = s.Coupon.Tutor.Id,
                        CouponType = s.Coupon.CouponType,
                        Code = s.Coupon.Code
                        
                    })
                    .Take(1)
                    .ToFutureValue();


                var paymentFuture = _stateless.Query<StudyRoomSessionUser>()
                    .Fetch(f => f.StudyRoomSession)
                    .ThenFetch(f => f.StudyRoom)
                    .ThenFetch(f => f.Tutor)
                    .ThenFetch(f => f.User)
                    .Fetch(f => f.User)
                    .Where(w => w.User.Id == query.UserId)
                    .Where(w => w.StudyRoomSession.Id == query.SessionId)
                    .Select(s => new PaymentDetailDto
                    {
                        TutorPricePerHour = s.PricePerHour,
                        UserId = s.User.Id,
                        TutorId = query.TutorId,
                        UserName = s.User.Name,
                        Created = s.StudyRoomSession.Created,
                        CantPay = s.StudyRoomSession.StudyRoom.Tutor.SellerKey == null,
                        _duration = s.TutorApproveTime ?? s.Duration,
                        TutorName = s.StudyRoomSession.StudyRoom.Tutor.User.Name

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

                return payment;


                //This query will not work in case there will be more then one student in a room.
                //           const string sql = @"select srs.Id as StudyRoomSessionId,
                //               case when t.Price is null then tr.Price else t.Price end as TutorPricePerHour,
                //               case when tr.SellerKey is null then 1 else 0 end as cantPay,
                //                 tr.Id as TutorId, 
                //                 tu.Name as TutorName, 
                //                 u.Id as UserId,
                //                 u.Name as UserName,
                //                 srs.Created,
                //		COALESCE (srs.RealDuration, srs.Duration) as DurationInTicks,

                //		x.CouponCode as CouponCode,
                //		x.couponType,
                //		x.CouponValue as CouponValue,
                //                       x.CouponTutor as CouponTutor

                //               from [sb].[StudyRoomSession] srs
                //               join sb.StudyRoom sr
                //                on srs.StudyRoomId = sr.Id
                //               left join sb.TutorHistory t
                //                on sr.TutorId = t.Id and srs.Created between t.BeginDate and t.EndDate
                //               join sb.Tutor tr
                //                on tr.Id = sr.TutorId
                //               join sb.StudyRoomUser sru
                //                on srs.StudyRoomId = sru.StudyRoomId and sru.userId != tr.Id
                //               join sb.[user] u
                //                on u.id = sru.UserId
                //outer apply (
                //Select c.code as CouponCode,
                //		c.couponType,
                //		c.Value as CouponValue,
                //                       c.tutorId as CouponTutor
                //		from  sb.userCoupon uc 
                //		join sb.coupon c on uc.couponId = c.id and uc.UsedAmount < c.AmountOfUsePerUser
                //			 where u.id = uc.userid and tr.id = uc.tutorId
                //) x
                //               join sb.[User] tu
                //                on tr.Id = tu.Id
                //               where srs.id = @id;";

                //           using var conn = _repository.OpenConnection();
                //           var result = await conn.QuerySingleAsync<PaymentDetailDto>(sql, new { id = query.SessionId });

                //           if (result.CouponType is null)
                //           {
                //               //no coupon
                //               return result;
                //           }

                //           if (result.CouponTutor.HasValue && result.CouponTutor.Value != result.TutorId)
                //           {
                //               return result;
                //           }

                //           result.StudentPayPerHour = Coupon.CalculatePrice(result.CouponType.Value, result.TutorPricePerHour, result.CouponValue.GetValueOrDefault());

                //           if (result.CouponTutor is null)
                //           {
                //               result.SpitballPayPerHour = result.TutorPricePerHour - result.StudentPayPerHour;
                //           }

                //           return result;
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