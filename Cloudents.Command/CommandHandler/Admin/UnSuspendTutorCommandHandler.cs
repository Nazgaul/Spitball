using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class UnSuspendTutorCommandHandler : ICommandHandler<UnSuspendTutorCommand>
    {
        private readonly IRepository<Tutor> _tutorRepository;

        public UnSuspendTutorCommandHandler(IRepository<Tutor> tutorRepository)
        {
            _tutorRepository = tutorRepository;
        }

        public async Task ExecuteAsync(UnSuspendTutorCommand message, CancellationToken token)
        {
            var tutor = await _tutorRepository.GetAsync(message.TutorId, token);
            tutor.UnSuspend();
        }
    }
}
