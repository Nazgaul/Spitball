using System;
using System.Linq;
using Cloudents.Core.Entities;
using Moq;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    public class CouponTests
    {
        [Fact]
        public void InitCoupon_NoCode_RaiseException()
        {
            Assert.Throws<ArgumentNullException>(() => new Coupon(null, CouponType.Flat, null, 50, null, 1, null, null, null));
        }

        [Fact]
        public void InitCoupon_CouponTypePercentageOver100_RaiseException()
        {
            Assert.Throws<ArgumentException>(() => new Coupon("SomeCode", CouponType.Percentage, null, 150, null, 1, null, null, null));
        }

        [Fact]
        public void InitCoupon_CouponTypeValueMinus_RaiseException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Coupon("SomeCode", CouponType.Percentage, null, -5, null, 1, null, null, null));
        }

        [Fact]
        public void InitCoupon_NegativeAmount_RaiseException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Coupon("SomeCode", CouponType.Percentage, null, 5, -1, 1, null, null, null));
        }


        [Fact]
        public void ApplyCoupon_ExpiredCoupon_RaiseException()
        {
            var coupon = new Coupon("SomeCode", CouponType.Percentage, null, 5, null, 1, null, null,
                null);

            typeof(Coupon).GetProperty("Expiration").SetValue(coupon, DateTime.UtcNow.AddDays(-1));

            var userMoq = new Mock<User>();
            var tutorMoq = new Mock<Tutor>();

            //couponMoq.Setup(x => x.Expiration).Returns(DateTime.UtcNow.AddDays(-1));
            Assert.Throws<ArgumentException>(() => coupon.ApplyCoupon(userMoq.Object, tutorMoq.Object));
        }

        [Fact]
        public void ApplyCoupon_AmountExceed_RaiseException()
        {
            var v = 10;
            var coupon = new Coupon("SomeCode", CouponType.Percentage, null, 5, v, 1, null, null,
                null);


            var tutorMoq = new Mock<Tutor>();
            var userMoq = new Mock<User>();
            var users = Enumerable.Range(0, v).Select(s =>
            {
                userMoq.Setup(s => s.Id).Returns(s);
                return userMoq;
            });
            foreach (var user in users)
            {
                coupon.ApplyCoupon(user.Object, tutorMoq.Object);
            }
            userMoq.Setup(s => s.Id).Returns(12);

            Assert.Throws<OverflowException>(() => coupon.ApplyCoupon(userMoq.Object, tutorMoq.Object));

        }
    }
}