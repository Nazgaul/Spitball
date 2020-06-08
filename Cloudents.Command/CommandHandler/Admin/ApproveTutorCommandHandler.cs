using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class ApproveTutorCommandHandler : ICommandHandler<ApproveTutorCommand>
    {
        private readonly IRepository<Tutor> _repository;
        public ApproveTutorCommandHandler(IRepository<Tutor> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(ApproveTutorCommand message, CancellationToken token)
        {
            var tutor = await _repository.LoadAsync(message.Id, token);

            tutor.Approve();
        }
    }

    public class BecomeTutorCommandHandler : ICommandHandler<BecomeTutorCommand>
    {
        private readonly IRegularUserRepository _repository;
        public BecomeTutorCommandHandler(IRegularUserRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(BecomeTutorCommand message, CancellationToken token)
        {
            var user = await _repository.LoadAsync(message.Id, token);

            user.BecomeTutor();
        }
    }
}
