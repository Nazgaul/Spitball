using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
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