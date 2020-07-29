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
        private readonly IRepository<Course> _courseRepository;

        public ApplyCouponCommandHandler(ICouponRepository couponRepository,
            IRegularUserRepository userRepository, ITutorRepository tutorRepository
            , IRepository<Course> studyRoomRepository)
        {
            _couponRepository = couponRepository;
            _userRepository = userRepository;
            _tutorRepository = tutorRepository;
            _courseRepository = studyRoomRepository;
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
            var course = await _courseRepository.LoadAsync(message.CourseId, token);

            message.NewPrice = Coupon.CalculatePrice(coupon.CouponType, course.Price.Amount, coupon.Value);

        }
    }
}