using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class DeleteTutorCommandHandler : ICommandHandler<DeleteTutorCommand>
    {
        private readonly ITutorRepository _tutorRepository;

        public DeleteTutorCommandHandler(ITutorRepository tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }

        public async Task ExecuteAsync(DeleteTutorCommand message, CancellationToken token)
        {
            Tutor tutorToRemove = await _tutorRepository.LoadAsync(message.Id, token);
            await _tutorRepository.DeleteAsync(tutorToRemove, token);
        }
    }
}
