using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Query.Tutor;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Courses
{
    public class CourseByIdQuery : IQuery<CourseDetailDto?>
    {
        private long Id { get; }

        private long UserId { get; }

        public CourseByIdQuery(long id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        internal sealed class CourseByIdQueryHandler : IQueryHandler<CourseByIdQuery, CourseDetailDto?>
        {
            private readonly IStatelessSession _statelessSession;
            private readonly IUrlBuilder _urlBuilder;

            public CourseByIdQueryHandler(IStatelessSession repository,
                IUrlBuilder urlBuilder)
            {
                _statelessSession = repository;
                _urlBuilder = urlBuilder;
            }

            public async Task<CourseDetailDto?> GetAsync(CourseByIdQuery query, CancellationToken token)
            {
                var futureCourse = _statelessSession.Query<Course>()
                    .Where(w => w.Id == query.Id && w.State == ItemState.Ok)
                    .Select(s => new CourseDetailDto
                    {
                        Id = s.Id,
                        TutorId = s.Tutor.Id,
                        Name = s.Name,
                        Price = s.Price,
                        SubscriptionPrice = s.SubscriptionPrice,
                        TutorName = s.Tutor.User.Name,
                        TutorImage = s.Tutor.User.ImageName,
                        TutorCountry = s.Tutor.User.SbCountry,
                        TutorSellerKey = s.Tutor.SellerKey,
                        Description = s.Description,
                        Details = s.Details,
                        TutorBio = s.Tutor.Paragraph2,
                        Version = s.Version
                    }).ToFutureValue();


                var futureStudyRoom = _statelessSession.Query<BroadCastStudyRoom>()
                    .Where(w => w.Course.Id == query.Id)
                    .OrderBy(o=>o.BroadcastTime)
                    //.Where(w => w.BroadcastTime > DateTime.UtcNow.AddHours(-1))
                    .Select(
                        s2 => new FutureBroadcastStudyRoomDto()
                        {
                            Id = s2.Id,
                            DateTime = s2.BroadcastTime,
                            Name = s2.Description,
                            OnGoing = _statelessSession.Query<StudyRoomSession>().Any(w => w.Ended == null && w.StudyRoom.Id == s2.Id)
                        }).ToFuture();


                var futureDocuments = _statelessSession.Query<Document>()
                    .Where(w => w.Course.Id == query.Id)
                    .Where(w => w.Status.State == ItemState.Ok)
                    .OrderBy(o=>o.Position).Select(s2 =>
                    new DocumentFeedDto()
                    {
                        Title = s2.Name,
                        DocumentType = s2.DocumentType,
                        Id = s2.Id,
                        Preview = _urlBuilder.BuildDocumentThumbnailEndpoint(s2.Id, null)
                    }).ToFuture();


                var futureCoupon  = _statelessSession.Query<UserCoupon>()
                    .Where(w => w.User.Id == query.UserId)
                    .Where(w => w.Tutor.Id == _statelessSession.Query<Course>().Where(w2 => w2.Id == query.Id)
                        .Select(s => s.Tutor.Id).First())
                    .Where(w => w.UsedAmount < 1)
                    .Select(s => new StudyRoomQuery.CouponTemp()
                    {
                        CouponType = s.Coupon.CouponType,
                        Value = s.Coupon.Value
                    })
                    .ToFutureValue();

                var futureSubscription = _statelessSession.Query<Follow>()
                    .Where(w => w.User.Id == _statelessSession.Query<Course>()
                        .Where(w2 => w2.Id == query.Id)
                        .Select(s => s.Tutor.Id).First())
                    .Where(w => w.Follower.Id == query.UserId)
                    .Select(s => s.Subscriber)
                    .ToFutureValue();

                var enrollmentsFuture = _statelessSession.Query<CourseEnrollment>()
                    .Where(a => a.User.Id == query.UserId && a.Course.Id == query.Id).ToFutureValue(f => f.Any());

                var fullFuture = _statelessSession.Query<CourseEnrollment>()
                    .Where(a => a.Course.Id == query.Id).ToFutureValue(f => f.Count());

                var sessionStartedFuture = _statelessSession.Query<StudyRoomSession>()
                    .Where(w => ((BroadCastStudyRoom) w.StudyRoom).Course.Id == query.Id && w.Ended == null)
                    .ToFutureValue(f => f.Any());

                var result = await futureCourse.GetValueAsync(token);

                if (result is null)
                {
                    return null;
                }

                result.StudyRooms = await futureStudyRoom.GetEnumerableAsync(token);
                result.Documents = await futureDocuments.GetEnumerableAsync(token);
                result.BroadcastTime = result.StudyRooms.DefaultIfEmpty().Min(m => m?.DateTime);
                result.Full = fullFuture.Value == 48;
                result.Enrolled = enrollmentsFuture.Value;
                result.SessionStarted = sessionStartedFuture.Value;

                




                if (futureSubscription.Value.GetValueOrDefault())
                {
                    result.Price = result.SubscriptionPrice.GetValueOrDefault(result.Price.ChangePrice(0));
                }

                var coupon = futureCoupon.Value;
                if (coupon != null)
                {
                    var newPrice = Coupon.CalculatePrice(coupon.CouponType,
                        result.Price.Amount, coupon.Value);
                    result.Price = result.Price.ChangePrice(newPrice);
                }

                return result;
            }
        }
    }
}