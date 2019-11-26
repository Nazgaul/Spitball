using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class ChangeTutorPriceCommandHandler : ICommandHandler<ChangeTutorPriceCommand>
    {
        private readonly ITutorRepository _tutorRepository;

        public ChangeTutorPriceCommandHandler(ITutorRepository tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }

        public async Task ExecuteAsync(ChangeTutorPriceCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.LoadAsync(message.TutorId, token);

            tutor.AdminChangePrice(message.Price);
        }
    }
}