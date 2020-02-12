﻿using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class CreateCouponCommandHandler : ICommandHandler<CreateCouponCommand>
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IRepository<Coupon> _couponRepository;

        const string description = "DashboardCoupon";

        public CreateCouponCommandHandler(ITutorRepository tutorRepository, IRepository<Coupon> couponRepository)
        {
            _tutorRepository = tutorRepository;
            _couponRepository = couponRepository;
        }

        public async Task ExecuteAsync(CreateCouponCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.LoadAsync(message.TutorId, token);
            var coupon = new Coupon(message.Code, message.CouponType, tutor, message.Value,
                null, 1, null, description, null);

            await _couponRepository.AddAsync(coupon, token);
        }
    }
}
