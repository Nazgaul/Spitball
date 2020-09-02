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
           
            var coupon = new Coupon(message.Code, message.CouponType, 
                message.Value, 
                message.Expiration
               );
            await _couponRepository.AddAsync(coupon, token);
        }
    }
}