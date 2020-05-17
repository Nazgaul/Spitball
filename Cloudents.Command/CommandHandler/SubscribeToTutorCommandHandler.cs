using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class SubscribeToTutorCommandHandler : ICommandHandler<SubscribeToTutorCommand>
    {

        private readonly ITutorRepository _tutorRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly IStripeService _stripeService;

        public SubscribeToTutorCommandHandler(ITutorRepository tutorRepository, IRegularUserRepository userRepository, IStripeService stripeService)
        {
            _tutorRepository = tutorRepository;
            _userRepository = userRepository;
            _stripeService = stripeService;
        }

        public async Task ExecuteAsync(SubscribeToTutorCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.LoadAsync(message.TutorId, token);
            var user = await _userRepository.LoadAsync(message.UserId, token);

          //  _stripeService.SubscribeToTutorAsync(tutor,user,)
        }
    }
}