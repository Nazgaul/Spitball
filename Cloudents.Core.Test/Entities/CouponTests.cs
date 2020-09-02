using System;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Moq;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    public class CouponTests
    {
        private readonly Tutor _tutor;

        public CouponTests()
        {
            var mock = new Mock<Tutor>();
            _tutor = mock.Object;
        }
        [Theory]
        [InlineData("a")]
        [InlineData("aaaaaaaaaaaaaaaaa")]
        public void InitCoupon_CodeNotInRange_RaiseException(string code)
        {

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _tutor.AddCoupon(code, CouponType.Flat, 50, null));


        }
        [Fact]
        public void InitCoupon_NoCode_RaiseException()
        {
            Assert.Throws<ArgumentNullException>(() => _tutor.AddCoupon(null!, CouponType.Flat, 50, null));
        }

        [Fact]
        public void InitCoupon_CouponTypePercentageOver100_RaiseException()
        {
            Assert.Throws<ArgumentException>(() => _tutor.AddCoupon("SomeCode", CouponType.Percentage, 150, null));
        }

        [Fact]
        public void InitCoupon_CouponTypeValueMinus_RaiseException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _tutor.AddCoupon("SomeCode", CouponType.Percentage, -5, null));
        }

        //[Fact]
        //public void InitCoupon_NegativeAmount_RaiseException()
        //{
        //    Assert.Throws<ArgumentOutOfRangeException>(() => new Coupon("SomeCode", CouponType.Percentage, null, 5, -1,  null));
        //}

        [Fact]
        public void CanApplyCoupon_ExpiredCoupon_RaiseException()
        {

            var couponMock = new Mock<Coupon>();

            couponMock.SetupProperty(s => s.Expiration, DateTime.UtcNow.AddDays(-1));
            //var coupon = new Coupon("SomeCode", CouponType.Percentage, null, 5, null,  
            //    null);

            // ReSharper disable once PossibleNullReferenceException
            //typeof(Coupon).GetProperty("Expiration").SetValue(coupon, DateTime.UtcNow.AddDays(-1));

            Assert.Throws<ArgumentException>(() => couponMock.Object.CanApplyCoupon());
        }

        [Fact]
        public void InitCoupon_Valid_Ok()
        {
            _tutor.AddCoupon("SomeCode", CouponType.Percentage, 5, null);



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