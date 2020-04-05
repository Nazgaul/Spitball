using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Session
{
    public class SessionApprovalQuery : IQuery<PaymentDetailDto?>
    {
        public SessionApprovalQuery(Guid sessionId, long userId, long tutorId)
        {
            SessionId = sessionId;
            UserId = userId;
            TutorId = tutorId;
        }

        private Guid SessionId { get; }
        private long UserId { get; }
        private long TutorId { get; }

        internal sealed class PaymentBySessionIdQueryHandler : IQueryHandler<SessionApprovalQuery, PaymentDetailDto?>
        {
            private readonly IStatelessSession _stateless;

            public PaymentBySessionIdQueryHandler(QuerySession querySession)
            {
                _stateless = querySession.StatelessSession;
            }

            public async Task<PaymentDetailDto?> GetAsync(SessionApprovalQuery query, CancellationToken token)
            {
                var couponFuture = _stateless.Query<UserCoupon>()
                     .Fetch(f => f.Coupon)
                     .Where(w => w.User.Id == query.UserId && w.Tutor.Id == query.TutorId)
                     .Where(w => w.UsedAmount < w.Coupon.AmountOfUsePerUser)
                     .Select(s => new
                     {
                         s.Coupon.Code,
                         s.Coupon.CouponType,
                         s.Coupon.Value,
                         //s.Coupon.Id
                     })
                     .Take(1)
                     .ToFutureValue();


               var studyRoomUserFuture =  _stateless.Query<StudyRoomSessionUser>()
                    .Fetch(f => f.StudyRoomSession)
                    .Where(w => w.User.Id == query.UserId)
                    .Where(w => w.StudyRoomSession.Id == query.SessionId)
                    .Select(s => new PaymentDetailDto
                    {
                        TutorPricePerHour = s.PricePerHour
                    }).ToFutureValue();


               var result = await studyRoomUserFuture.GetValueAsync(token);
               if (result is null)
               {
                   return null;
               }
               var couponResult = couponFuture.Value;
               if (couponResult != null)
               {
                   result.CouponType = couponResult.CouponType;
                   result.CouponCode = couponResult.Code;
                   result.CouponValue = couponResult.Value;
                   result.CouponTutor = query.TutorId;
               }

               return result;
            }
        }
    }
}