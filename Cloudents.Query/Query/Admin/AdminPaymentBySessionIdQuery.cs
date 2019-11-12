using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Dapper;

namespace Cloudents.Query.Query.Admin
{
    public class AdminPaymentBySessionIdQuery : IQuery<PaymentDetailDto>
    {
        public AdminPaymentBySessionIdQuery(Guid sessionId)
        {
            SessionId = sessionId;
        }

        private Guid SessionId { get; }

        internal sealed class AdminPaymentBySessionIdQueryHandler : IQueryHandler<AdminPaymentBySessionIdQuery, PaymentDetailDto>
        {
            private readonly IDapperRepository _repository;

            public AdminPaymentBySessionIdQueryHandler(IDapperRepository repository)
            {
                _repository = repository;
            }

            public async Task<PaymentDetailDto> GetAsync(AdminPaymentBySessionIdQuery query, CancellationToken token)
            {
                //This query will not work in case there will be more then one student in a room.
                string sql = @"select srs.Id as StudyRoomSessionId,
                    case when t.Price is null then tr.Price else t.Price end as TutorPricePerHour,
                    case when tr.SellerKey is null then 1 else 0 end as cantPay,
		                    tr.Id as TutorId, 
		                    tu.Name as TutorName, 
		                    u.Id as UserId,
		                    u.Name as UserName,
		                    srs.Created,
							datediff(MINUTE, srs.Created, srs.Ended) as Duration,
							
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
								 where u.id = uc.userid and t.id = uc.tutorId
					) x
                    join sb.[User] tu
	                    on tr.Id = tu.Id
                    where srs.id = @id";

                using (var conn = _repository.OpenConnection())
                {
                    var result = await conn.QuerySingleAsync<PaymentDetailDto>(sql, new { id = query.SessionId });

                    if (result.CouponType is null)
                    {
                        //no coupon
                        return result;
                    }

                    if (result.CouponTutor.HasValue && result.CouponTutor.Value != result.TutorId)
                    {
                        return result;
                    }

                    result.StudentPayPerHour = Coupon.CalculatePrice(result.CouponType.Value, result.TutorPricePerHour, result.CouponValue.GetValueOrDefault());

                    if (result.CouponTutor is null)
                    {
                        result.SpitballPayPerHour = result.TutorPricePerHour - result.StudentPayPerHour;
                    }

                    return result;
                    //  public decimal StudentPay { get; set; }
                    //public decimal SpitballPay { get; set; }

                }
            }
        }
    }
}