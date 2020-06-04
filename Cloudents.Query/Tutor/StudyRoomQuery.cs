using Cloudents.Core.DTOs;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using PaymentStatus = Cloudents.Core.Enum.PaymentStatus;

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

            public StudyRoomQueryHandler(QuerySession repository)
            {
                _statelessSession = repository.StatelessSession;
            }

            public async Task<StudyRoomDto?> GetAsync(StudyRoomQuery query, CancellationToken token)
            {
                //TODO: make it better
                //  using var conn = _repository.OpenConnection();

                //var studyRoomSessionFuture = _statelessSession.Query<StudyRoomSession>()
                //    .WithOptions(w => w.SetComment(nameof(StudyRoomSession)))
                //    .Where(w => w.StudyRoom.Id == query.Id && w.Ended == null && w.Created > DateTime.UtcNow.AddHours(-6))
                //    .OrderByDescending(o => o.Id).Take(1).ToFutureValue();

                var futureStudyRoom = _statelessSession.Query<StudyRoom>()
                     .Fetch(f => f.Tutor)
                     .ThenFetch(f => f.User)
                     .Where(w => w.Id == query.Id)
                     .Select(s => new StudyRoomDto
                     {
                         OnlineDocument = s.OnlineDocumentUrl,
                         ConversationId = s.Identifier,
                         TutorId = s.Tutor.Id,
                         BroadcastTime = ((BroadCastStudyRoom)s).BroadcastTime,
                         Type = s is BroadCastStudyRoom ? StudyRoomType.Broadcast : StudyRoomType.Private,
                         Name = s.Name,
                         TutorPrice = s.Price,
                         TutorName = s.Tutor.User.Name,
                         TutorImage = s.Tutor.User.ImageName,
                         _UserPaymentExists =
                             _statelessSession.Query<User>().Where(w => w.Id == query.UserId)
                                 .Select(s2 => s2.PaymentExists).First() == PaymentStatus.Done,
                         TutorCountry = s.Tutor.User.SbCountry ?? Country.UnitedStates
                     }).ToFutureValue();

                var futureCoupon = _statelessSession.Query<UserCoupon>()
                    .Where(w => w.User.Id == query.UserId)
                    .Where(w => w.Tutor.Id == _statelessSession.Query<StudyRoom>().Where(w => w.Id == query.Id)
                        .Select(s => s.Tutor.Id).First())
                    .Where(w => w.UsedAmount < 1)
                    .Select(s => new { s.Coupon.CouponType, s.Coupon.Value })
                    .ToFutureValue();



                var result = await futureStudyRoom.GetValueAsync(token);

                if (result is null)
                {
                    return null;
                }

                if (result.BroadcastTime.HasValue && result.BroadcastTime.Value < DateTime.UtcNow.AddHours(-6))
                {
                    return null;
                }

                result.UserId = query.UserId;


                var coupon = futureCoupon.Value;
                if (coupon is null)
                {
                    //no coupon
                    return result;
                }


                result.TutorPrice = Coupon.CalculatePrice(coupon.CouponType,
                                  result.TutorPrice, coupon.Value);



                //if (result.TutorPrice.CompareTo(0) == 0)
                //{
                //    result.NeedPayment = false;
                //}

                return result;
            }
        }
    }
}