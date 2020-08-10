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

            public StudyRoomQueryHandler(IStatelessSession repository)
            {
                _statelessSession = repository;
            }

            public async Task<StudyRoomDto?> GetAsync(StudyRoomQuery query, CancellationToken token)
            {
                var futureStudyRoom = _statelessSession.Query<StudyRoom>()
                     .Fetch(f => f.Tutor)
                     .ThenFetch(f => f.User)
                     .Where(w => w.Id == query.Id)
                     .Select(s => new StudyRoomDto
                     {
                         Enrolled = _statelessSession.Query<StudyRoomUser>().Any(w => w.Room.Id == query.Id && w.User.Id ==query.UserId),
                         OnlineDocument = s.OnlineDocumentUrl,
                         ConversationId = s.Identifier,
                         TutorId = s.Tutor.Id,
                         BroadcastTime = ((BroadCastStudyRoom)s).BroadcastTime,
                         Type = s is BroadCastStudyRoom ? StudyRoomType.Broadcast : StudyRoomType.Private,
                         TopologyType = s.TopologyType,
                         Name = ((BroadCastStudyRoom)s).Course.Name ?? ((PrivateStudyRoom)s).Name,
                         TutorPrice = s.Price,
                         TutorName = s.Tutor.User.Name,
                         TutorImage = s.Tutor.User.ImageName,
                         
                         _UserPaymentExists =
                             _statelessSession.Query<User>().Where(w => w.Id == query.UserId)
                                 .Select(s2 => s2.PaymentExists).First() == PaymentStatus.Done,
                         TutorCountry = s.Tutor.User.SbCountry
                     }).ToFutureValue();

                IFutureValue<CouponTemp>? futureCoupon = null;
                IFutureValue<CourseEnrollment>? futureCourseEnrollment = null;
                if (query.UserId > 0)
                {
                    futureCoupon  = _statelessSession.Query<UserCoupon>()
                        .Where(w => w.User.Id == query.UserId)
                        .Where(w => w.Tutor.Id == _statelessSession.Query<StudyRoom>().Where(w2 => w2.Id == query.Id)
                            .Select(s => s.Tutor.Id).First())
                        .Where(w => w.UsedAmount < 1)
                        .Select(s => new CouponTemp()
                        {
                            CouponType = s.Coupon.CouponType,
                           Value = s.Coupon.Value
                        })
                        .ToFutureValue();

                    futureCourseEnrollment = _statelessSession.Query<CourseEnrollment>()
                        .Where(w => w.User.Id == query.UserId)
                        .Where(w => w.Course.Id == _statelessSession.Query<BroadCastStudyRoom>()
                            .Where(w2 => w2.Id == query.Id)
                            .Select(s => s.Course.Id).First()).ToFutureValue();
                }

                var futurePayment = _statelessSession.Query<StudyRoomPayment>()
                    .Where(w => w.StudyRoom!.Id == query.Id && w.User.Id == query.UserId)
                    
                    .ToFutureValue(f=>f.Any());


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

                if (result.BroadcastTime.HasValue && result.BroadcastTime.Value < DateTime.UtcNow.AddHours(-6))
                {
                    return null;
                }

                result.UserId = query.UserId;
                if (result.TutorId == query.UserId)
                {
                    result.Enrolled = true;
                }

                if (futurePayment.Value)
                {
                    result._UserPaymentExists = true;
                }
                
                if (futureSubscription.Value.GetValueOrDefault())
                {
                    result.TutorPrice = result.TutorPrice.ChangePrice(0);
                }

                if (futureCourseEnrollment?.Value?.Receipt != null)
                {
                    result._UserPaymentExists = true;

                    return result;
                }


                var coupon = futureCoupon?.Value;
                if (coupon is null)
                {
                    //no coupon
                    return result;
                }


                var newPrice = Coupon.CalculatePrice(coupon.CouponType,
                    result.TutorPrice.Amount, coupon.Value);
                result.TutorPrice = new Money(newPrice, result.TutorPrice.Currency);

                return result;
            }
        }
        internal class CouponTemp
        {
            public CouponType CouponType { get; set; }
            public decimal Value { get; set; }
        }
    }
}