using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    public class CouponMap : ClassMap<Coupon>
    {
        public CouponMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Code).Not.Nullable();
            Map(x => x.CouponType).Not.Nullable();
            
            References(x => x.Tutor).Nullable();
            References(x => x.Course).ForeignKey("FK_COUPON_COURSE");

            Map(x => x.Value).Column("Value2").Not.Nullable();
           // Map(x => x.ValueOld).Column("Value").Not.Nullable();

            Map(x => x.Expiration).Nullable();
            Map(x => x.Description).Nullable().Length(8000);
            Map(x => x.CreateTime).Insert().Not.Update();

            HasMany(x => x.UserCoupons).Access.CamelCaseField(Prefix.Underscore)
                .Inverse().Cascade.AllDeleteOrphan().AsSet();
        }
    }

    public class UserCouponMap : ClassMap<UserCoupon>
    {
        public UserCouponMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User).Not.Nullable();
            References(x => x.Coupon).Not.Nullable();

            Map(x => x.Price).CustomType<MoneyCompositeUserType>();
            //References(x => x.Tutor).Not.Nullable();
            //We get signature-of-the-body-and-declaration-in-a-method-implementation-do-not-match if its lazy load on User.ApplyCoupon method
            //References(x => x.StudyRoomSessionUser).LazyLoad(Laziness.False)
            //    .Column("SessionUserId").Nullable();
            //Map(x => x.UsedAmount);
            Map(x => x.CreatedTime);
        }
    }
}