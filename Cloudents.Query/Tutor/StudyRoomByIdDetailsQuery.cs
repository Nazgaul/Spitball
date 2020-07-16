using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Tutor
{
    public class StudyRoomByIdDetailsQuery : IQuery<StudyRoomDetailDto?>
    {
        private Guid Id { get; }

        private long UserId { get; }
        public StudyRoomByIdDetailsQuery(Guid id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        internal sealed class StudyRoomByIdDetailsQueryHandler : IQueryHandler<StudyRoomByIdDetailsQuery, StudyRoomDetailDto?>
        {
            private readonly IStatelessSession _statelessSession;

            public StudyRoomByIdDetailsQueryHandler(IStatelessSession repository)
            {
                _statelessSession = repository;
            }

            public async Task<StudyRoomDetailDto?> GetAsync(StudyRoomByIdDetailsQuery query, CancellationToken token)
            {
                var futureStudyRoom = _statelessSession.Query<BroadCastStudyRoom>()
                    .Fetch(f => f.Tutor)
                    .ThenFetch(f => f.User)
                    .Where(w => w.Id == query.Id)
                    .Select(s => new StudyRoomDetailDto
                    {
                        Enrolled = _statelessSession.Query<StudyRoomUser>().Any(w => w.Room.Id == query.Id && w.User.Id ==query.UserId),
                        TutorId = s.Tutor.Id,
                        BroadcastTime = s.BroadcastTime,
                        Name = s.Name,
                        Price = s.Price,
                        TutorName = s.Tutor.User.Name,
                        TutorImage = s.Tutor.User.ImageName,
                        TutorCountry = s.Tutor.User.SbCountry,
                        Description = s.Description,
                        Full = _statelessSession.Query<StudyRoomUser>().Count(w => w.Room.Id == query.Id) == 48,
                        Id = s.Id,
                        TutorBio = s.Tutor.Paragraph2
                    }).ToFutureValue();

                //IFutureValue<CouponTemp>? futureCoupon = null;
                //if (query.UserId > 0)
                //{
                //    futureCoupon  = _statelessSession.Query<UserCoupon>()
                //        .Where(w => w.User.Id == query.UserId)
                //        .Where(w => w.Tutor.Id == _statelessSession.Query<StudyRoom>().Where(w2 => w2.Id == query.Id)
                //            .Select(s => s.Tutor.Id).First())
                //        .Where(w => w.UsedAmount < 1)
                //        .Select(s => new CouponTemp()
                //        {
                //            CouponType = s.Coupon.CouponType,
                //            Value = s.Coupon.Value
                //        })
                //        .ToFutureValue();
                //}

                //var futurePayment = _statelessSession.Query<StudyRoomPayment>()
                //    .Where(w => w.StudyRoom!.Id == query.Id && w.User.Id == query.UserId)
                //    .ToFutureValue(f=>f.Any());


                var futureSubscription = _statelessSession.Query<Follow>()
                    .Where(w => w.User.Id == _statelessSession.Query<StudyRoom>().Where(w2 => w2.Id == query.Id)
                        .Select(s => s.Tutor.Id).First())
                    .Where(w => w.Follower.Id == query.UserId)
                    .Select(s => s.Subscriber)
                    .ToFutureValue();



                var result = await futureStudyRoom.GetValueAsync(token);

                if (result is null)
                {
                    return null;
                }

                if (result.BroadcastTime  < DateTime.UtcNow.AddHours(-6))
                {
                    return null;
                }

                //result.UserId = query.UserId;

                //if (futurePayment.Value)
                //{
                //    result._UserPaymentExists = true;
                //}

                if (futureSubscription.Value.GetValueOrDefault())
                {
                    result.Price = result.Price.ChangePrice(0);
                }


                //var coupon = futureCoupon?.Value;
                //if (coupon is null)
                //{
                //    //no coupon
                //    return result;
                //}


                //var newPrice = Coupon.CalculatePrice(coupon.CouponType,
                //    result.TutorPrice.Amount, coupon.Value);
                //result.TutorPrice = new Money(newPrice, result.TutorPrice.Currency);

                return result;
            }
        }
    }
}