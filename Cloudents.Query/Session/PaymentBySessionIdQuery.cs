using Cloudents.Core.DTOs;
using Dapper;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Session
{

    /// <summary>
    /// This is old version of study room session
    /// </summary>
    public class PaymentBySessionIdQuery : IQuery<PaymentDetailDto>
    {
        public PaymentBySessionIdQuery(Guid sessionId)
        {
            SessionId = sessionId;
        }

        private Guid SessionId { get; }

        internal sealed class PaymentBySessionIdQueryHandler : IQueryHandler<PaymentBySessionIdQuery, PaymentDetailDto>
        {
            private readonly IDapperRepository _repository;

            public PaymentBySessionIdQueryHandler(IDapperRepository repository)
            {
                _repository = repository;
            }

            public async Task<PaymentDetailDto> GetAsync(PaymentBySessionIdQuery query, CancellationToken token)
            {
                //This query will not work in case there will be more then one student in a room.
                const string sql = @"select case when t.Price is null then tr.Price else t.Price end as TutorPricePerHour,						
							x.CouponCode as CouponCode,
							x.couponType,
							x.CouponValue as CouponValue,
                            x.CouponTutor as CouponTutor
							
                    from [sb].[StudyRoomSession] srs
                    join sb.StudyRoom sr
	                    on srs.StudyRoomId = sr.Id
                    left join sb.TutorHistory t
	                    on sr.TutorId = t.Id and srs.Created between t.BeginDate and t.EndDate
                    join sb.Tutor tr
	                    on tr.Id = sr.TutorId
                    join sb.StudyRoomUser sru
	                    on srs.StudyRoomId = sru.StudyRoomId and sru.userId != tr.Id
                    join sb.[user] u
	                    on u.id = sru.UserId
					outer apply (
					Select c.code as CouponCode,
							c.couponType,
							c.Value as CouponValue,
                            c.tutorId as CouponTutor
							from  sb.userCoupon uc 
							join sb.coupon c on uc.couponId = c.id and uc.UsedAmount < c.AmountOfUsePerUser
								 where u.id = uc.userid and tr.id = uc.tutorId
					) x
                    join sb.[User] tu
	                    on tr.Id = tu.Id
                    where srs.id = @id";

                using var conn = _repository.OpenConnection();
                var result = await conn.QuerySingleAsync<PaymentDetailDto>(sql, new { id = query.SessionId });
                return result;
            }
        }
    }
}
