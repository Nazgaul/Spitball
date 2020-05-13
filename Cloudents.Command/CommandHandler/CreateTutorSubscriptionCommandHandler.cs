using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class CreateTutorSubscriptionCommandHandler : ICommandHandler<CreateTutorSubscriptionCommand>
    {
        private readonly ITutorRepository _tutorRepository;

        public CreateTutorSubscriptionCommandHandler(ITutorRepository tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }

        public async Task ExecuteAsync(CreateTutorSubscriptionCommand message, CancellationToken token)
        {
            var tutor = _tutorRepository.LoadAsync(message.TutorId, token);
            throw new System.NotImplementedException();
        }
    }
}