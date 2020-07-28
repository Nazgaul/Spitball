using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class CourseEnrollCommandHandler : ICommandHandler<CourseEnrollCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<Course> _courseRepository;


        public CourseEnrollCommandHandler(IRegularUserRepository userRepository,
            IRepository<Course> courseRepository)
        {
            _userRepository = userRepository;
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(CourseEnrollCommand message, CancellationToken token)
        {
            //the same as enter study room
            var course = await _courseRepository.GetAsync(message.CourseId, token);
            if (course == null)
            {
                throw new NotFoundException();
            }

            if (course.State != ItemState.Ok)
            {
                throw new NotFoundException();
            }

            var user = await _userRepository.LoadAsync(message.UserId, token);
            if (course.Price.Cents == 0)
            {
                course.EnrollUser(user,null,course.Price);
                return;
            }

            var price = course.Price;
            var subscription = course.Tutor.User.Followers
                .SingleOrDefault(s => s.Follower.Id == message.UserId)?.Subscriber;
            if (subscription == true)
            {
                price = course.SubscriptionPrice ?? price.ChangePrice(0);

            }

            if (price.Cents == 0)
            {
                course.EnrollUser(user,null,price);
                return;
            }

            var coupon = course.Tutor.UserCoupons.FirstOrDefault(f => f.User.Id == message.UserId && f.IsNotUsed());

            if (coupon != null)
            {
                var tempPrice = Coupon.CalculatePrice(coupon.Coupon.CouponType, price.Amount,
                    coupon.Coupon.Value);

                price = price.ChangePrice(tempPrice);
            }
            if (price.Cents == 0)
            {
                course.EnrollUser(user,null,price);
                return;
            }

            if (message.Receipt == null)
            {
                throw new UnauthorizedAccessException();
            }

            //Need to continue
            course.EnrollUser(user,message.Receipt,price);

            //if (studyRoom.Price.Cents > 0 &&
            //    studyRoom.Tutor.User.SbCountry != Country.Israel && message.Receipt == null)
            //{
            //    var coupon = studyRoom.Tutor.UserCoupons.FirstOrDefault(f => f.User.Id == message.UserId && f.IsNotUsed());

            //    var tempPrice = 100d;
            //    if (coupon != null)
            //    {
            //        tempPrice = Coupon.CalculatePrice(coupon.Coupon.CouponType, studyRoom.Price.Amount,
            //            coupon.Coupon.Value);
            //    }

            //    if (tempPrice > 0)
            //    {
            //        var subscription = studyRoom.Tutor.User.Followers
            //            .SingleOrDefault(s => s.Follower.Id == message.UserId)?.Subscriber;
            //        if (subscription.GetValueOrDefault() == false)
            //        {
            //            throw new UnauthorizedAccessException();
            //        }
            //    }
            //}
            

            //if (message.Receipt != null)
            //{
            //    studyRoom.AddPayment(user, message.Receipt);
            //}

            //studyRoom.AddUserToStudyRoom(user);

        }
    }
}