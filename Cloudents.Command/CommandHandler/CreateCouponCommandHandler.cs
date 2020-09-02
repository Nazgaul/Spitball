using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class CreateCouponCommandHandler : ICommandHandler<CreateCouponCommand>
    {
        private readonly ITutorRepository _tutorRepository;


        public CreateCouponCommandHandler(ITutorRepository tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }

        public async Task ExecuteAsync(CreateCouponCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.LoadAsync(message.TutorId, token);
            tutor.AddCoupon(message.Code,message.CouponType,message.Value,message.Expiration);

        }
    }
}
