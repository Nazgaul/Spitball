using System;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Cloudents.Command.Command;
using Cloudents.Command.CommandHandler;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cloudents.Core.Test.CommandHandler
{
    public class ApplyCouponCommandHandlerTests : IDisposable
    {
        private readonly AutoMock _mock;

        public ApplyCouponCommandHandlerTests()
        {
            _mock = AutoMock.GetLoose();
        }

        [Fact]
        public async Task ExecuteAsync_InvalidCoupon_Error()
        {
            const string someCoupon = "xxxxxx";
            _mock.Mock<ICouponRepository>().Setup(s => s.GetCouponAsync(someCoupon, default)).ReturnsAsync((Coupon)null);

            var command = new ApplyCouponCommand(someCoupon, 0, 0);
            var instance = _mock.Create<ApplyCouponCommandHandler>();
            await Assert.ThrowsAsync<ArgumentException>(() => instance.ExecuteAsync(command, default));

        }


        [Fact]
        public async Task ExecuteAsync_ExpiredCoupon_Error()
        {
            //var couponMock = new Mock<Coupon>();

            var coupon = new Coupon("vvvvvv", CouponType.Flat,
                null, 10, 1, 1, DateTime.UtcNow.AddYears(-1),
                null, null);

            var user = new User("someUser","firstName","lastName", Language.English,"IN");


            // var userMoq = new Mock<User>();
            var tutorMoq = new Mock<Tutor>();
            tutorMoq.Setup(s => s.Price.Price).Returns(555);
            //couponMock.Setup(x => x.Expiration).Returns(DateTime.UtcNow.AddDays(-1));
            const string someCoupon = "xxxxxx";
            _mock.Mock<IRegularUserRepository>().Setup(s => s.LoadAsync(1L, default)).ReturnsAsync(user);
            _mock.Mock<ITutorRepository>().Setup(s => s.LoadAsync(2L, default)).ReturnsAsync(tutorMoq.Object);

            _mock.Mock<ICouponRepository>().Setup(s => s.GetCouponAsync(someCoupon, default)).ReturnsAsync(coupon);
            var command = new ApplyCouponCommand(someCoupon, 1, 2);
            var instance = _mock.Create<ApplyCouponCommandHandler>();
            await Assert.ThrowsAsync<ArgumentException>(() => instance.ExecuteAsync(command, default));
        }


        [Fact]
        public async Task ExecuteAsync_CouponNotBelongToTutor_Error()
        {
            var couponMock = new Mock<Coupon>();
            var userMoq = new Mock<User>();
            var tutorMoq = new Mock<Tutor>();
            var belongTutorMoq = new Mock<Tutor>();
            belongTutorMoq.Setup(x => x.Id).Returns(1000);

            tutorMoq.Setup(s => s.Price.Price).Returns(555);
            couponMock.Setup(x => x.Tutor).Returns(belongTutorMoq.Object);
            const string someCoupon = "xxxxxx";

            _mock.Mock<IRegularUserRepository>().Setup(s => s.LoadAsync(1L, default)).ReturnsAsync(userMoq.Object);
            _mock.Mock<ITutorRepository>().Setup(s => s.LoadAsync(2L, default)).ReturnsAsync(tutorMoq.Object);

            _mock.Mock<ICouponRepository>().Setup(s => s.GetCouponAsync(someCoupon, default)).ReturnsAsync(couponMock.Object);
            var command = new ApplyCouponCommand(someCoupon, 1, 2);
            var instance = _mock.Create<ApplyCouponCommandHandler>();
            await Assert.ThrowsAsync<ArgumentException>(() => instance.ExecuteAsync(command, default));
        }

        [Theory]
        [InlineData(100, 2, 98)]
        [InlineData(2, 50, 0)]
        public async Task ExecuteAsync_FlatCalculation_Ok(decimal price, decimal discount, decimal total)
        {

            const string someCoupon = "xxxxxx";

            long tutorId = 1, userId = 2;
            //var userMoq = new Mock<User>();
            var user = new User("SomeEmail", "firstName", "lastName", Language.English,"IN");
            typeof(User).GetProperty("Id").SetValue(user, userId);
            var tutorMoq = new Mock<Tutor>();

            // userMoq.Setup(s => s.Id).Returns(userId);
            tutorMoq.Setup(s => s.Price.Price).Returns(price);
            tutorMoq.Setup(s => s.Id).Returns(tutorId);

            var coupon = new Coupon(someCoupon, CouponType.Flat, null, discount, null, 1, null, null, null);
            _mock.Mock<ICouponRepository>().Setup(s => s.GetCouponAsync(someCoupon, default)).ReturnsAsync(coupon);
            _mock.Mock<ITutorRepository>().Setup(s => s.LoadAsync(tutorId, default)).ReturnsAsync(tutorMoq.Object);
            _mock.Mock<IRegularUserRepository>().Setup(s => s.LoadAsync(userId, default)).ReturnsAsync(user);
            var command = new ApplyCouponCommand(someCoupon, userId, tutorId);
            var instance = _mock.Create<ApplyCouponCommandHandler>();
            await instance.ExecuteAsync(command, default);
            command.newPrice.Should().Be(total);

        }

        [Theory]
        [InlineData(100, 2, 98)]
        [InlineData(2, 50, 1)]
        [InlineData(2, 100, 0)]
        public async Task ExecuteAsync_PercentageCalculation_Ok(decimal price, decimal discount, decimal total)
        {

            const string someCoupon = "xxxxxx";

            long tutorId = 1, userId = 2;
            //var userMoq = new Mock<User>();
            var user = new User("SomeEmail", "firstName", "lastName", Language.English,"IN");
            typeof(User).GetProperty("Id").SetValue(user, userId);

            var tutorMoq = new Mock<Tutor>();
           // userMoq.Setup(s => s.Id).Returns(userId);
            tutorMoq.Setup(s => s.Price.Price).Returns(price);
            tutorMoq.Setup(s => s.Id).Returns(tutorId);

            var coupon = new Coupon(someCoupon, CouponType.Percentage, null, discount, null, 1, null, null, null);

            _mock.Mock<ICouponRepository>().Setup(s => s.GetCouponAsync(someCoupon, default)).ReturnsAsync(coupon);
            _mock.Mock<ITutorRepository>().Setup(s => s.LoadAsync(tutorId, default)).ReturnsAsync(tutorMoq.Object);
            _mock.Mock<IRegularUserRepository>().Setup(s => s.LoadAsync(userId, default)).ReturnsAsync(user);
            var command = new ApplyCouponCommand(someCoupon, userId, tutorId);
            var instance = _mock.Create<ApplyCouponCommandHandler>();
            await instance.ExecuteAsync(command, default);
            command.newPrice.Should().Be(total);

        }


        public void Dispose()
        {
            _mock.Dispose();
        }
    }
}