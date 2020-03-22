using Cloudents.Core.DTOs;
using Dapper;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;

namespace Cloudents.Query.Tutor
{
    public class StudyRoomQuery : IQuery<StudyRoomDto?>
    {
        public StudyRoomQuery(Guid id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        private Guid Id { get; }

        private long UserId { get; }

        internal sealed class StudyRoomQueryHandler : IQueryHandler<StudyRoomQuery, StudyRoomDto?>
        {
            //private readonly IDapperRepository _repository;
            private readonly IStatelessSession _statelessSession;
            private readonly IVideoProvider _videoProvider;

            public StudyRoomQueryHandler(QuerySession repository, IVideoProvider videoProvider)
            {
                _videoProvider = videoProvider;
                _statelessSession = repository.StatelessSession;
            }

            public async Task<StudyRoomDto?> GetAsync(StudyRoomQuery query, CancellationToken token)
            {
                //TODO: make it better
                //  using var conn = _repository.OpenConnection();

                var studyRoomSessionFuture = _statelessSession.Query<StudyRoomSession>()
                    .Where(w => w.StudyRoom.Id == query.Id && w.Ended == null)
                    .OrderByDescending(o => o.Id).Take(1).ToFutureValue();



                var sqlQuery = _statelessSession.CreateSQLQuery(@"
Select 
onlineDocumentUrl as OnlineDocument, 
sr.identifier as ConversationId,
sr.tutorId,
t.Price as TutorPrice,
u.Name as TutorName,
u.ImageName as TutorImage,
u1.Id as StudentId, u1.Name as StudentName, u1.ImageName as StudentImage,
x.*,
 coalesce (
	case when t.price = 0 then 0 else null end,
	case when u1.PaymentExists = 1 then 0 else null end,
    case when u1.Country = 'IN' then 0 else null end,
    case when EXISTS (select top 1 * from sb.UserToken where userid = :UserId and state = 'NotUsed') then 0 else null end,
	1
) as NeedPayment
from sb.StudyRoom sr 
join sb.Tutor t on t.Id = sr.TutorId
join sb.[User] u on t.Id = u.Id
join sb.StudyRoomUser sru1 on sr.Id = sru1.StudyRoomId and sru1.UserId != sr.TutorId
join sb.StudyRoomUser sru2 on sr.Id = sru2.StudyRoomId and sru2.UserId = :UserId
join sb.[user] u1 on sru1.UserId = u1.Id
outer apply (
					Select 
							c.couponType,
							c.Value as CouponValue
                           	from  sb.userCoupon uc 
							join sb.coupon c on uc.couponId = c.id and uc.UsedAmount < c.AmountOfUsePerUser
								 where :UserId = uc.userid and t.id = uc.tutorId
					) x
where sr.id = :Id;");

                sqlQuery.SetGuid("Id", query.Id);
                sqlQuery.SetInt64("UserId", query.UserId);

                sqlQuery.SetResultTransformer(Transformers.AliasToBean<StudyRoomDto>());
                var resultFuture = sqlQuery.FutureValue<StudyRoomDto>();

                var result = await resultFuture.GetValueAsync(token);

                var studyRoomSession = studyRoomSessionFuture.Value;


                //.Where(w=>w.StudyRoom.Id == query.Id && w.Ended == null)
                //                var result =  await conn.QuerySingleOrDefaultAsync<StudyRoomDto>(@"
                //Select 
                //onlineDocumentUrl as OnlineDocument, 
                //sr.identifier as ConversationId,
                //sr.tutorId,
                //t.Price as TutorPrice,
                //u.Name as TutorName,
                //u.ImageName as TutorImage,
                //u1.Id as StudentId, u1.Name as StudentName, u1.ImageName as StudentImage,
                //x.*,
                // coalesce (
                //	case when t.price = 0 then 0 else null end,
                //	case when u1.PaymentExists = 1 then 0 else null end,
                //    case when u1.Country = 'IN' then 0 else null end,
                //    case when EXISTS (select top 1 * from sb.UserToken where userid = @userid and state = 'NotUsed') then 0 else null end,
                //	1
                //) as NeedPayment
                //from sb.StudyRoom sr 
                //join sb.Tutor t on t.Id = sr.TutorId
                //join sb.[User] u on t.Id = u.Id
                //join sb.StudyRoomUser sru1 on sr.Id = sru1.StudyRoomId and sru1.UserId != sr.TutorId
                //join sb.StudyRoomUser sru2 on sr.Id = sru2.StudyRoomId and sru2.UserId = @UserId
                //join sb.[user] u1 on sru1.UserId = u1.Id
                //outer apply (
                //					Select 
                //							c.couponType,
                //							c.Value as CouponValue
                //                           	from  sb.userCoupon uc 
                //							join sb.coupon c on uc.couponId = c.id and uc.UsedAmount < c.AmountOfUsePerUser
                //								 where @UserId = uc.userid and t.id = uc.tutorId
                //					) x
                //where sr.id = @Id;",
                //                    new { query.Id, query.UserId });

                if (result is null)
                {
                    return null;
                }
                if (studyRoomSession != null)
                {
                    var jwt = _videoProvider.CreateRoomToken(studyRoomSession.SessionId, query.UserId);
                    result.Jwt = jwt;
                }
                if (result.CouponType is null)
                {
                    //no coupon
                    return result;
                }



                result.TutorPrice = Coupon.CalculatePrice(result.CouponType.Value,
                    result.TutorPrice, result.CouponValue.GetValueOrDefault());

                if (result.TutorPrice == 0)
                {
                    result.NeedPayment = false;
                }

                return result;
            }
        }
    }
}