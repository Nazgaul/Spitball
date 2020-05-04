using System;
using Cloudents.Query.Stuff;
using NHibernate;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate.Transform;
using NHibernate.Linq;
using Cloudents.Core.DTOs.Users;
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
                const string sql = @"select u.id,
u.ImageName as Image,
u.Name,
u.description as Tutor_Description,
u.online,
cast ((select count(*) from sb.GoogleTokens gt where u.Id = gt.Id) as bit) as CalendarShared,
u.FirstName as FirstName,
u.LastName as LastName,
u.CoverImage as Cover,
t.price as Tutor_Price, 
t.SubsidizedPrice as Tutor_DiscountPrice,
t.country as Tutor_TutorCountry,
t.rate as Tutor_Rate,
t.rateCount as Tutor_ReviewCount,
t.Bio as Tutor_Bio,
t.Lessons as Tutor_Lessons,
(select count(*) from sb.document d where d.userid = u.id and d.state = 'Ok')
+ (select count(*) from sb.Question d where d.userid = u.id and d.state = 'Ok')
as Tutor_ContentCount,
(select count(distinct UserId) from sb.StudyRoom sr
join sb.StudyRoomUser sru on sr.Id = sru.StudyRoomId
where sr.TutorId = :profileId and sru.UserId != :profileId) as Tutor_Students,
(select count(1) from sb.UsersRelationship where UserId = u.Id) as Followers,
case when exists (select * from sb.UsersRelationship ur where ur.UserId = :profileId and ur.FollowerId = :userid) then cast(1 as bit) else cast(0 as bit) end as IsFollowing
from sb.[user] u 
left join sb.readTutor t 
	on U.Id = t.Id 
where u.id = :profileId
and (u.LockoutEnd is null or u.LockoutEnd < GetUtcDate())";



                var sqlQuery = _session.CreateSQLQuery(sql);
                sqlQuery.SetInt64("profileId", query.Id);
                sqlQuery.SetInt64("userid", query.UserId);
                sqlQuery.SetResultTransformer(new DeepTransformer<UserProfileDto>('_'));
                var profileValue = sqlQuery.FutureValue<UserProfileDto>();


                var couponQuery = _session.Query<UserCoupon>()
                      .Where(w => w.User.Id == query.UserId)
                      .Where(w => w.Tutor.Id == query.Id)
                      .Where(w => w.UsedAmount < w.Coupon.AmountOfUsePerUser)
                      .Select(s => new CouponDto
                      {
                          Value = s.Coupon.Value,
                          TypeEnum = s.Coupon.CouponType
                      }).ToFutureValue();



                var futureSubject = _session.Query<ReadTutor>().Where(t => t.Id == query.Id)
                    .Select(s => s.Subjects).ToFutureValue();

                var documentCoursesFuture = _session.Query<Document>()
                    .Fetch(f => f.User)
                    .Where(w => w.User.Id == query.Id && w.Status.State == Core.Enum.ItemState.Ok)
                    .Select(s => s.Course.Id).Distinct()
                    .ToFuture();


                var userCoursesFuture = _session.Query<UserCourse>()
                    .Where(w => w.User.Id == query.Id)
                    .Take(20)
                    .Select(s => s.Course.Id).ToFuture();



                var result = await profileValue.GetValueAsync(token);

                var couponResult = couponQuery.Value;

                if (result is null)
                {
                    return null;
                }

                if (result.Tutor != null)
                {
                    result.Tutor.Subjects = futureSubject.Value;
                    if (couponResult != null)
                    {
                        result.Tutor.CouponType = couponResult.TypeEnum;
                        result.Tutor.CouponValue = couponResult.Value;

                    }
                }

                result.DocumentCourses = documentCoursesFuture.GetEnumerable();
                result.Courses = userCoursesFuture.GetEnumerable();
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

        public class CouponDto
        {
            public CouponType TypeEnum { get; set; }

            // public CouponType TypeEnum => (CouponType)Enum.Parse(typeof(CouponType), Type, true);

            public decimal Value { get; set; }
        }

    }
}