using Cloudents.Query.Stuff;
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
using NHibernate.SqlCommand;

namespace Cloudents.Query.Users
{
    public class UserProfileQuery : IQuery<UserProfileDto?>
    {
        //TODO split to two queries
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

            public UserProfileQueryHandler(QuerySession session, IUrlBuilder urlBuilder)
            {
                _urlBuilder = urlBuilder;
                _session = session.StatelessSession;
            }

            public async Task<UserProfileDto?> GetAsync(UserProfileQuery query, CancellationToken token)
            {
            
                var userFuture = _session.Query<BaseUser>()
                      .Where(w => w.Id == query.Id)
                      .Select(s => new UserProfileDto()
                      {
                          Id = s.Id,
                          Image = s.ImageName,
                          Name = s.Name,
                          Online = ((User)s).Online,
                          CalendarShared = _session.Query<GoogleTokens>().Any(w => w.Id == query.Id.ToString()),
                          FirstName = ((User)s).FirstName,
                          LastName = ((User)s).LastName,
                          Cover = ((User)s).CoverImage,
                          Followers = _session.Query<Follow>().Count(w => w.Followed.Id == query.Id),
                          IsFollowing = _session.Query<Follow>()
                              .Any(w => w.Followed.Id == query.Id && w.Follower.Id == query.UserId)
                      }).ToFutureValue();

                var tutorFuture = _session.Query<ReadTutor>()
                    .Where(w => w.Id == query.Id)
                    .Select(s => new UserTutorProfileDto()
                    {
                        Price = s.Price,
                        DiscountPrice = s.SubsidizedPrice,
                        TutorCountry = s.SbCountry,
                        Rate = s.Rate.GetValueOrDefault(),
                        ReviewCount = s.RateCount,
                        Bio = s.Bio,
                        Lessons = s.Lessons,
                        
                        ContentCount = _session.Query<Document>()
                                           .Count(w => w.Status.State == ItemState.Ok && w.User.Id == query.Id) +
                                       _session.Query<Question>().Count(w =>
                                           w.Status.State == ItemState.Ok && w.User.Id == query.Id),
                        Students = _session.Query<StudyRoomUser>()
                            .Where(w => w.Room.Tutor.Id == query.Id).Select(s=>s.User.Id).Distinct().Count(),
                        SubscriptionPrice = s.SubscriptionPrice,
                        Subjects = s.Subjects,
                        Description = s.Description
                    }).ToFutureValue();


                var couponQuery = _session.Query<UserCoupon>()
                      .Where(w => w.User.Id == query.UserId)
                      .Where(w => w.Tutor.Id == query.Id)
                      .Where(UserCoupon.IsUsedExpression)
                      .Select(s => new CouponDto
                      {
                          Value = s.Coupon.Value,
                          TypeEnum = s.Coupon.CouponType
                      }).ToFutureValue();



                //var futureSubject = _session.Query<ReadTutor>().Where(t => t.Id == query.Id)
                //    .Select(s => s.Subjects).ToFutureValue();

                var documentCoursesFuture = _session.Query<Document>()
                    .Fetch(f => f.User)
                    .Where(w => w.User.Id == query.Id && w.Status.State == Core.Enum.ItemState.Ok)
                    .Select(s => s.Course.Id).Distinct()
                    .ToFuture();


                var userCoursesFuture = _session.Query<UserCourse>()
                    .Where(w => w.User.Id == query.Id)
                    .Take(20)
                    .Select(s => s.Course.Id).ToFuture();

                var result = await userFuture.GetValueAsync(token);
             

                //var result = await profileValue.GetValueAsync(token);

               

                if (result is null)
                {
                    return null;
                }

                var tutorValue = tutorFuture.Value;
                result.Tutor = tutorValue;
                var couponResult = couponQuery.Value;

                if (result.Tutor != null)
                {
                    //result.Tutor.Subjects = futureSubject.Value;
                    if (couponResult != null)
                    {
                        result.Tutor.CouponType = couponResult.TypeEnum;
                        result.Tutor.CouponValue = couponResult.Value;

                    }
                }

                result.DocumentCourses = await documentCoursesFuture.GetEnumerableAsync(token);
                result.Courses = await userCoursesFuture.GetEnumerableAsync(token);
                result.Image = _urlBuilder.BuildUserImageEndpoint(result.Id, result.Image);
                result.Cover = _urlBuilder.BuildUserImageEndpoint(result.Id, result.Cover);

                if (result.Tutor?.CouponValue != null && result.Tutor?.CouponType != null)
                {
                    result.Tutor.HasCoupon = true;
                    result.Tutor.DiscountPrice = Coupon.CalculatePrice(result.Tutor.CouponType.Value, result.Tutor.Price,
                        result.Tutor.CouponValue.Value);
                }
                return result;
            }
        }

        private class CouponDto
        {
            public CouponType TypeEnum { get; set; }
            public decimal Value { get; set; }
        }

    }
}