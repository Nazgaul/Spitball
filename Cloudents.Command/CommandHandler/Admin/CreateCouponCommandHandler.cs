using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class CreateCouponCommandHandler : ICommandHandler<CreateCouponCommand>
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IRepository<Coupon> _couponRepository;

        public CreateCouponCommandHandler(ITutorRepository tutorRepository, IRepository<Coupon> couponRepository)
        {
            _tutorRepository = tutorRepository;
            _couponRepository = couponRepository;
        }

        public async Task ExecuteAsync(CreateCouponCommand message, CancellationToken token)
        {
            Tutor? tutor = null;
            if (message.TutorId.HasValue)
            {
                tutor = await _tutorRepository.LoadAsync(message.TutorId.Value, token);
            }
            var coupon = new Coupon(message.Code, message.CouponType, tutor,
                message.Value, 
                //message.AmountOfUsers,
                //message.AmountOfUsePerUser,
                message.Expiration,
                message.Description);
            await _couponRepository.AddAsync(coupon, token);
        }
    }
}