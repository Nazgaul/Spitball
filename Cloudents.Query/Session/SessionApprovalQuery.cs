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
        public SessionApprovalQuery(Guid sessionId, long tutorId)
        {
            SessionId = sessionId;
           // UserId = userId;
            TutorId = tutorId;
        }

        private Guid SessionId { get; }
        //private long UserId { get; }
        private long TutorId { get; }

        internal sealed class SessionApprovalQueryHandler : IQueryHandler<SessionApprovalQuery, PaymentDetailDto?>
        {
            private readonly IStatelessSession _stateless;

            public SessionApprovalQueryHandler(IStatelessSession querySession)
            {
                _stateless = querySession;
            }

            public async Task<PaymentDetailDto?> GetAsync(SessionApprovalQuery query, CancellationToken token)
            {
                //var couponFuture = _stateless.Query<UserCoupon>()
                //     .Fetch(f => f.Coupon)
                //     .Where(w => w.StudyRoomSessionUser!.Id == query.SessionId)
                //     .Select(s => new
                //     {
                //         s.Coupon.Code,
                //         s.Coupon.CouponType,
                //         s.Coupon.Value,
                //     })
                //     .ToFutureValue();


                var studyRoomUserFuture =  _stateless.Query<StudyRoomPayment>()
                    .Where(w => w.Id == query.SessionId)
                    .Select(s => new PaymentDetailDto
                    {
                        TutorPricePerHour = s.PricePerHour
                    }).ToFutureValue();


               var result = await studyRoomUserFuture.GetValueAsync(token);
               if (result is null)
               {
                   return null;
               }
               //var couponResult = couponFuture.Value;
               //if (couponResult != null)
               //{
               //    result.CouponType = couponResult.CouponType;
               //    result.CouponCode = couponResult.Code;
               //    result.CouponValue = couponResult.Value;
               //    result.CouponTutor = query.TutorId;
               //}

               return result;
            }
        }
    }
}