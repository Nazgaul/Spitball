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

        public SubscribeToTutorCommandHandler(ITutorRepository tutorRepository, IRegularUserRepository userRepository)
        {
            _tutorRepository = tutorRepository;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(SubscribeToTutorCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.LoadAsync(message.TutorId, token);
            var user = await _userRepository.LoadAsync(message.UserId, token);

            tutor.User.AddSubscriber(user);
          //  _stripeService.SubscribeToTutorAsync(tutor,user,)
        }
    }
}