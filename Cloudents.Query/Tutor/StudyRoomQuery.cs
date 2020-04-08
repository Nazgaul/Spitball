using Cloudents.Core.DTOs;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Query.Stuff;
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
                    .WithOptions(w => w.SetComment(nameof(StudyRoomSession)))
                    .Where(w => w.StudyRoom.Id == query.Id && w.Ended == null && w.Created > DateTime.UtcNow.AddHours(-6))
                    .OrderByDescending(o => o.Id).Take(1).ToFutureValue();

                var sqlQuery = _statelessSession.CreateSQLQuery(@"
DECLARE @Id UNIQUEIDENTIFIER = :Id, @UserId int = :UserId
DECLARE @True bit = 1, @False bit = 0;
Select 
onlineDocumentUrl as OnlineDocument, 
sr.identifier as ConversationId,
sr.tutorId,
sr.BroadcastTime,
sr.StudyRoomType as Type,
sr.Name,
COALESCE(sr.Price,t.Price) as TutorPrice,
u.Name as TutorName,
u.ImageName as TutorImage,
x.*,
  coalesce (
    case when sr.price = 0 then @False else null end,
	case when t.price = 0 then @False else null end,
    case when t.id = @UserId then @False else null end ,
	case when COALESCE( (select u2.PaymentExists from sb.[user] u2 where id = @UserId),0) = 1 then @False else null end,
    case when u.Country = 'IN' then @False else null end,
    case when EXISTS (select top 1 * from sb.UserToken ut where userid = @UserId and  
(state = 'NotUsed' or  ut.created >  DATEADD(Minute,-30,GETUTCDATE())) and @Id = ut.studyRoomId) then @False else null end,
	@True
) as NeedPayment
from sb.StudyRoom sr 
join sb.Tutor t on t.Id = sr.TutorId
join sb.[User] u on t.Id = u.Id
outer apply (
					Select 
							c.couponType,
							c.Value as CouponValue
                           	from  sb.userCoupon uc 
							join sb.coupon c on uc.couponId = c.id and uc.UsedAmount < c.AmountOfUsePerUser
								 where @UserId = uc.userid and t.id = uc.tutorId
					) x
where sr.id = @Id;");

                sqlQuery.SetGuid("Id", query.Id);
                sqlQuery.SetInt64("UserId", query.UserId);

                sqlQuery.SetResultTransformer(new SbAliasToBeanResultTransformer<StudyRoomDto>());
                var resultFuture = sqlQuery.FutureValue<StudyRoomDto>();

                var result = await resultFuture.GetValueAsync(token);

                var studyRoomSession = studyRoomSessionFuture.Value;

                if (result is null)
                {
                    return null;
                }

                if (result.BroadcastTime.HasValue && result.BroadcastTime.Value < DateTime.UtcNow)
                {
                    return null;
                }
                if (studyRoomSession != null)
                {
                    var roomAvailable = await _videoProvider.GetRoomAvailableAsync(studyRoomSession.SessionId);
                    if (roomAvailable)
                    {
                        var jwt = _videoProvider.CreateRoomToken(studyRoomSession.SessionId, query.UserId);
                        result.Jwt = jwt;
                    }
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