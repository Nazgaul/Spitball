using NHibernate;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate.Linq;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Enum;

namespace Cloudents.Query.Users
{
    public class UserProfileQuery : IQuery<UserProfileDto?>
    {
        public UserProfileQuery(long id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        private long Id { get; }
        private long UserId { get; }



        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
        internal sealed class UserProfileQueryHandler : IQueryHandler<UserProfileQuery, UserProfileDto?>
        {

            private readonly IStatelessSession _session;
            private readonly IUrlBuilder _urlBuilder;

            public UserProfileQueryHandler(IStatelessSession session, IUrlBuilder urlBuilder)
            {
                _urlBuilder = urlBuilder;
                _session = session;
            }

            public async Task<UserProfileDto?> GetAsync(UserProfileQuery query, CancellationToken token)
            {
                var tutorFuture = _session.Query<Core.Entities.Tutor>()
                    .Fetch(f => f.User)
                    .Where(w => w.Id == query.Id)
                    .Select(s => new UserProfileDto()
                    {
                        Id = s.Id,
                        Image = s.User.ImageName,
                        CalendarShared = _session.Query<GoogleTokens>().Any(w => w.Id == query.Id.ToString()),
                        FirstName = s.User.FirstName,
                        LastName = s.User.LastName,
                        Cover = s.User.CoverImage,
                        Followers = _session.Query<Follow>().Count(w => w.User.Id == query.Id),
                        TutorCountry = s.User.SbCountry,
                        Bio = s.Paragraph2,
                        //ContentCount = _session.Query<Document>()
                        //                   .Count(w => w.Status.State == ItemState.Ok && w.User.Id == query.Id) +
                        //               _session.Query<Question>().Count(w =>
                        //                   w.Status.State == ItemState.Ok && w.User.Id == query.Id),
                        Students = _session.Query<StudyRoomUser>()
                            .Where(w => w.Room.Tutor.Id == query.Id).Select(s2 => s2.User.Id).Distinct().Count(),
                        SubscriptionPrice = s.SubscriptionPrice,
                        Description = s.Title
                    }).ToFutureValue();


                var lessonsQuery = _session.Query<StudyRoomSession>()
                    .Fetch(f => f.StudyRoom)
                    .Where(w => w.StudyRoom.Tutor.Id == query.Id)
                    .ToFutureValue(f => f.Count());


                //TODO
                //Lessons => Total hours
                //Students => Amount of files

                var rateQuery = _session.Query<TutorReview>()
                    .Where(w => w.Tutor.Id == query.Id)
                    .GroupBy(g=>1)
                    .Select(f =>new
                    {
                        Count = f.Count(),
                        Total = f.Sum(s=>(float?)s.Rate)
                    })
                    .ToFutureValue();

                //var couponQuery = _session.Query<UserCoupon>()
                //      .Where(w => w.User.Id == query.UserId)
                //      .Where(w => w.Tutor.Id == query.Id)
                //      .Where(UserCoupon.IsUsedExpression)
                //      .Select(s => new CouponDto
                //      {
                //          Value = s.Coupon.Value,
                //          TypeEnum = s.Coupon.CouponType
                //      }).ToFutureValue();

                var documentCoursesFuture = _session.Query<Document>()
                    .Fetch(f => f.User)
                    .Where(w => w.User.Id == query.Id && w.Status.State == ItemState.Ok)
                    .Select(s => s.Course.Id).Distinct()
                    .ToFuture();



                //var userCoursesFuture = _session.Query<UserCourse>()
                //    .Where(w => w.User.Id == query.Id)
                //    .Take(20)
                //    .Select(s => s.Course.Id).ToFuture();


                var isFollowingFuture = _session.Query<Follow>()
                    .Where(w => w.User.Id == query.Id && w.Follower.Id == query.UserId).ToFutureValue();

                var result = await tutorFuture.GetValueAsync(token);

                if (result is null)
                {
                    return null;
                }

                //var tutorValue = tutorFuture.Value;
                // result.Tutor = tutorValue;
                //  var couponResult = couponQuery.Value;
                var isFollowing = isFollowingFuture.Value;
                result.IsFollowing = isFollowing != null;
                result.Lessons = lessonsQuery.Value;
                result.ReviewCount = rateQuery.Value.Count;
                result.Rate = rateQuery.Value.Total.GetValueOrDefault() / rateQuery.Value.Count;
                //result.Tutor.Subjects = futureSubject.Value;
                //if (couponResult != null)
                //{
                //    result.Tutor.CouponType = couponResult.TypeEnum;
                //    result.Tutor.CouponValue = couponResult.Value;

                //}

                result.IsSubscriber = isFollowing?.Subscriber ?? false;


                result.DocumentCourses = await documentCoursesFuture.GetEnumerableAsync(token);
                //result.Courses = await userCoursesFuture.GetEnumerableAsync(token);
                result.Image = _urlBuilder.BuildUserImageEndpoint(result.Id, result.Image);
                result.Cover = _urlBuilder.BuildUserImageEndpoint(result.Id, result.Cover);

                //if (result.Tutor?.CouponValue != null && result.Tutor?.CouponType != null)
                //{
                //    result.Tutor.HasCoupon = true;
                //    //result.Tutor.DiscountPrice = Coupon.CalculatePrice(result.Tutor.CouponType.Value, result.Tutor.Price,
                //    //    result.Tutor.CouponValue.Value);
                //}
                return result;
            }
        }

        //private class CouponDto
        //{
        //    public CouponType TypeEnum { get; set; }
        //    public decimal Value { get; set; }
        //}

    }
}