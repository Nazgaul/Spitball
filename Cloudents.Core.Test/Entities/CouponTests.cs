using System;
using System.Linq;
using System.Reflection;
using Cloudents.Core.Entities;
using Moq;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    public class CouponTests
    {
        [Theory]
        [InlineData("a")]
        [InlineData("aaaaaaaaaaaaaaaaa")]
        public void InitCoupon_CodeNotInRange_RaiseException(string code)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Coupon(code, CouponType.Flat, null, 50, null,  null));

        }
        [Fact]
        public void InitCoupon_NoCode_RaiseException()
        {
            Assert.Throws<ArgumentNullException>(() => new Coupon(null, CouponType.Flat, null, 50, null,  null));
        }

        [Fact]
        public void InitCoupon_CouponTypePercentageOver100_RaiseException()
        {
            Assert.Throws<ArgumentException>(() => new Coupon("SomeCode", CouponType.Percentage, null, 150, null,  null));
        }

        [Fact]
        public void InitCoupon_CouponTypeValueMinus_RaiseException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Coupon("SomeCode", CouponType.Percentage, null, -5, null,  null));
        }

        //[Fact]
        //public void InitCoupon_NegativeAmount_RaiseException()
        //{
        //    Assert.Throws<ArgumentOutOfRangeException>(() => new Coupon("SomeCode", CouponType.Percentage, null, 5, -1,  null));
        //}

        [Fact]
        public void CanApplyCoupon_ExpiredCoupon_RaiseException()
        {
            var coupon = new Coupon("SomeCode", CouponType.Percentage, null, 5, null,  
                null);

            // ReSharper disable once PossibleNullReferenceException
            typeof(Coupon).GetProperty("Expiration").SetValue(coupon, DateTime.UtcNow.AddDays(-1));

            Assert.Throws<ArgumentException>(() => coupon.CanApplyCoupon());
        }

        [Fact]
        public void InitCoupon_Valid_Ok()
        {
            var _ = new Coupon("SomeCode", CouponType.Percentage, null, 5, null, 
                null);

            
        }

        //[Fact]
        //public void CanApplyCoupon_AmountExceed_RaiseException()
        //{
        //    var amountOfUserOfCoupon = 10;
        //    var coupon = new Coupon("SomeCode", CouponType.Percentage, null, 5, amountOfUserOfCoupon, 
        //        null);



        //    var usedCoupon = Enumerable.Range(0, amountOfUserOfCoupon).Select(s =>
        //    {
        //        var x = new Mock<UserCoupon>();
        //        return x.Object;
        //    }).ToHashSet();



        //    // ReSharper disable once PossibleNullReferenceException
        //    typeof(Coupon).GetProperty("UserCoupon", BindingFlags.NonPublic | BindingFlags.Instance)
        //        .SetValue(coupon, usedCoupon);
            
            

        //    Assert.Throws<OverflowException>(() => coupon.CanApplyCoupon());

        //}

        
    }
}