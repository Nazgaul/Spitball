using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class SuspendTutorCommandHandler : ICommandHandler<SuspendTutorCommand>
    {
        private readonly IRepository<Tutor> _tutorRepository;

        public SuspendTutorCommandHandler(IRepository<Tutor> tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }

        public async Task ExecuteAsync(SuspendTutorCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.GetAsync(message.TutorId, token);
            tutor.Suspend();
        }
    }
}
