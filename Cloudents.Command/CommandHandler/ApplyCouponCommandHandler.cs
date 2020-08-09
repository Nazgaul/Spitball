using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class ApplyCouponCommandHandler : ICommandHandler<ApplyCouponCommand>
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IRegularUserRepository _userRepository;

        public ApplyCouponCommandHandler(ICouponRepository couponRepository,
            IRegularUserRepository userRepository)
        {
            _couponRepository = couponRepository;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(ApplyCouponCommand message, CancellationToken token)
        {
            var coupon = await _couponRepository.GetCouponAsync(message.Coupon, token);

            if (coupon is null)
            {
                throw new ArgumentException("invalid coupon");
            }

            if (coupon.Course.Id  !=  message.CourseId)
            {
                throw new ArgumentException("invalid coupon");
            }

            
            var user = await _userRepository.LoadAsync(message.UserId, token);
            coupon.ApplyCoupon(user);

            message.NewPrice = coupon.CalculatePrice();

        }
    }
}