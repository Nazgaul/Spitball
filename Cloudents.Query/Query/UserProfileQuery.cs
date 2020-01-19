using System;
using Cloudents.Core.DTOs;
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

namespace Cloudents.Query.Query
{
    public class UserProfileQuery : IQuery<UserProfileDto>
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
        internal sealed class UserProfileQueryHandler : IQueryHandler<UserProfileQuery, UserProfileDto>
        {

            private readonly IStatelessSession _session;
            private readonly IUrlBuilder _urlBuilder;

            public UserProfileQueryHandler(QuerySession session, IUrlBuilder urlBuilder)
            {
                _urlBuilder = urlBuilder;
                _session = session.StatelessSession;
            }

            public async Task<UserProfileDto> GetAsync(UserProfileQuery query, CancellationToken token)
            {

                const string sql = @"select u.id,
u.ImageName as Image,
u.Name,
u2.name as universityName,
u.description,
u.online,
cast ((select count(*) from sb.GoogleTokens gt where u.Id = gt.Id) as bit) as CalendarShared,

u.FirstName as FirstName,
u.LastName as LastName,
t.price as Tutor_Price, 
t.SubsidizedPrice as Tutor_DiscountPrice,
t.country as Tutor_TutorCountry,
t.rate as Tutor_Rate,
t.rateCount as Tutor_ReviewCount,
t.Bio as Tutor_Bio,
t.Lessons as Tutor_Lessons,
(select count(*) from sb.document d where d.userid = u.id and d.state = 'Ok') as Tutor_Documents,
(select count(1) from sb.UsersRelationship where UserId = u.Id) as Followers,
case when exists (select * from sb.UsersRelationship ur where ur.UserId = :profileId and ur.FollowerId = :userid) then cast(1 as bit) else cast(0 as bit) end as IsFollowing
from sb.[user] u 
left join sb.[University] u2 on u.UniversityId2 = u2.Id
left join sb.readTutor t 
	on U.Id = t.Id 
where u.id = :profileId
and (u.LockoutEnd is null or u.LockoutEnd < GetUtcDate())";


                const string couponSql = @"Select c.value as Value,
c.CouponType as Type
from sb.UserCoupon uc
join sb.coupon c on uc.couponId = c.id and uc.UsedAmount < c.AmountOfUsePerUser
where userid = :userid
and uc.tutorId =  :profileId";

                var sqlQuery = _session.CreateSQLQuery(sql);
                sqlQuery.SetInt64("profileId", query.Id);
                sqlQuery.SetInt64("userid", query.UserId);
                sqlQuery.SetResultTransformer(new DeepTransformer<UserProfileDto>('_'));
                var profileValue = sqlQuery.FutureValue<UserProfileDto>();



                var couponSqlQuery = _session.CreateSQLQuery(couponSql);
                couponSqlQuery.SetInt64("profileId", query.Id);
                couponSqlQuery.SetInt64("userid", query.UserId);
                // couponSqlQuery.AddScalar("Type", NHibernateUtil.Enum(typeof(CouponType)));
                couponSqlQuery.SetResultTransformer(Transformers.AliasToBean<CouponDto>());
                var couponValue = couponSqlQuery.FutureValue<CouponDto>();


                var future = _session.Query<ReadTutor>().Where(t => t.Id == query.Id)
                    .Select(s => s.AllSubjects).ToFutureValue();

                var coursesFuture = _session.Query<UserCourse>()
                    .Fetch(f => f.User).Fetch(f => f.Course)
                    .Where(w => w.User.Id == query.Id)
                    .Select(s => s.Course.Id)
                    .ToFuture();


               
                var result = await profileValue.GetValueAsync(token);

                var couponResult = couponValue.Value;
                var coursesRedult = await coursesFuture.GetEnumerableAsync(token);

                if (result is null)
                {
                    return null;
                }

                if (result.Tutor != null)
                {
                    result.Tutor.Subjects = future.Value;
                    if (couponResult != null)
                    {
                        result.Tutor.CouponType = couponResult.TypeEnum;
                        result.Tutor.CouponValue = couponResult.Value;
                       
                    }
                }

                result.Courses = coursesRedult;

                result.Image = _urlBuilder.BuildUserImageEndpoint(result.Id, result.Image);

                if (result.Tutor?.CouponValue.HasValue == true && result.Tutor?.CouponType.HasValue == true)
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
            public string Type { get; set; }

            public CouponType TypeEnum => (CouponType)Enum.Parse(typeof(CouponType), Type, true);

            public decimal Value { get; set; }
        }

    }
}