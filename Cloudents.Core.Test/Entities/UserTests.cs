using System;
using System.Linq;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cloudents.Core.Test.Entities
{
    public class UserTests
    {


        [Fact]
        public void ApplyCoupon_AlreadyHaveTheSameOne_OnlyOne()
        {
            var user = new User("some email", "firstName", "lastName", Language.English,"IL");
            typeof(User).GetProperty("Id").SetValue(user, 1L);
            //user.Id = 1;


            var couponMoq1 = new Mock<Coupon>();
            couponMoq1.Setup(s => s.CanApplyCoupon()).Returns(true);
            var couponMoq2 = new Mock<Coupon>();
            couponMoq2.Setup(s => s.CanApplyCoupon()).Returns(true);

            var tutorMoq = new Mock<Tutor>();
            user.ApplyCoupon(couponMoq1.Object, tutorMoq.Object);
            Assert.Throws<DuplicateRowException>(() => user.ApplyCoupon(couponMoq2.Object, tutorMoq.Object));

            user.UserCoupon.Should().HaveCount(1);
        }

        [Fact]
        public void ApplyCoupon_HaveUsedCoupon_Two()
        {
            var user = new User("some email", "firstName", "lastName", Language.English,"IL");
            typeof(User).GetProperty("Id").SetValue(user, 1L);
            //user.Id = 1;
            var couponCode = "SomeCode";
            var couponMoq1Id = Guid.NewGuid();

            var couponMoq1 = new Mock<Coupon>();
            couponMoq1.Setup(s => s.CanApplyCoupon()).Returns(true);
            couponMoq1.Setup(s => s.Id).Returns(couponMoq1Id);
            couponMoq1.Setup(s => s.AmountOfUsePerUser).Returns(1);

            var couponMoq2 = new Mock<Coupon>();
            var couponMoq2Id = Guid.NewGuid();
            couponMoq2.Setup(s => s.Id).Returns(couponMoq2Id);
            couponMoq2.Setup(s => s.Code).Returns(couponCode);
            couponMoq2.Setup(s => s.CanApplyCoupon()).Returns(true);

            var tutorMoq = new Mock<Tutor>();
            tutorMoq.Setup(s => s.Id).Returns(50);
            user.ApplyCoupon(couponMoq1.Object, tutorMoq.Object);
            user.UseCoupon(tutorMoq.Object);

            user.ApplyCoupon(couponMoq2.Object, tutorMoq.Object);

            user.UserCoupon.Should().HaveCount(2);
            //user.UserCoupon.Single().Coupon.Code.Should().BeEquivalentTo(couponCode);
        }

        [Fact]
        public void ApplyCoupon_HaveNotUsedCoupon_Override()
        {
            var user = new User("some email", "firstName", "lastName", Language.English,"IL");
            typeof(User).GetProperty("Id").SetValue(user, 1L);
            //user.Id = 1;
            var couponCode = "SomeCode";

            var couponMoq1 = new Mock<Coupon>();
            var couponMoq1Id = Guid.NewGuid();
            couponMoq1.Setup(s => s.CanApplyCoupon()).Returns(true);
            couponMoq1.Setup(s => s.Id).Returns(couponMoq1Id);
            couponMoq1.Setup(s => s.AmountOfUsePerUser).Returns(1);

            var couponMoq2 = new Mock<Coupon>();
            var couponMoq2Id = Guid.NewGuid();
            couponMoq2.Setup(s => s.Id).Returns(couponMoq2Id);
            couponMoq2.Setup(s => s.Code).Returns(couponCode);
            couponMoq2.Setup(s => s.CanApplyCoupon()).Returns(true);

            //var tutor = new Tutor("xxx",new User("some email",Language.English), 200);
            //typeof(Tutor).GetProperty("Id").SetValue(tutor, 50L);
            var tutorMoq = new Mock<Tutor>();
            tutorMoq.Setup(s => s.Id).Returns(50);
            user.ApplyCoupon(couponMoq1.Object, tutorMoq.Object);
            //user.UseCoupon(tutorMoq.Object);

            Assert.Throws<DuplicateRowException>(() => user.ApplyCoupon(couponMoq2.Object, tutorMoq.Object));


            // user.UserCoupon.Should().HaveCount(1);
            //user.UserCoupon.Single().Coupon.Code.Should().BeEquivalentTo(couponCode);
        }

        //[Fact]
        //public void ApplyCoupon_HaveOneAlready_OverrideExisting()
        //{
        //    var coupon = new Coupon("SomeCode", CouponType.Percentage, null, 5, null, 1, null, null,
        //        null);
        //    var coupon2 = new Coupon("SomeCode", CouponType.Percentage, null, 5, null, 1, null, null,
        //        null);
        //    var userMoq = new Mock<User>();
        //    userMoq.Setup(s => s.Id).Returns(1);
        //    var tutorMoq = new Mock<Tutor>();
        //    tutorMoq.Setup(s => s.Id).Returns(2);

        //    coupon.ApplyCoupon(userMoq.Object, tutorMoq.Object);
        //    coupon2.ApplyCoupon(userMoq.Object, tutorMoq.Object);



        //    //Assert.Throws<ArgumentException>(() => coupon.ApplyCoupon(userMoq.Object, tutorMoq.Object));
        //}


    }
}