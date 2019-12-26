using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class ApplyCouponCommandHandler : ICommandHandler<ApplyCouponCommand>
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly ITutorRepository _tutorRepository;

        public ApplyCouponCommandHandler(ICouponRepository couponRepository,
            IRegularUserRepository userRepository, ITutorRepository tutorRepository)
        {
            _couponRepository = couponRepository;
            _userRepository = userRepository;
            _tutorRepository = tutorRepository;
        }

        public async Task ExecuteAsync(ApplyCouponCommand message, CancellationToken token)
        {
            var coupon = await _couponRepository.GetCouponAsync(message.Coupon, token);
            var tutor = await _tutorRepository.LoadAsync(message.TutorId, token);

            if (coupon is null)
            {
                throw new ArgumentException("invalid coupon");
            }

            if (coupon.Tutor != null && coupon.Tutor.Id != message.TutorId)
            {
                throw new ArgumentException("invalid coupon");
            }
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.ApplyCoupon(coupon, tutor);


            var tutorPrice = tutor.Price.Price;

            message.NewPrice = Coupon.CalculatePrice(coupon.CouponType, tutorPrice, coupon.Value);

        }
    }
}