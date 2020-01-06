using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class UpdateEmailCommandHandler : ICommandHandler<UpdateEmailCommand>
    {
        private readonly IRegularUserRepository _repository;
        public UpdateEmailCommandHandler(IRegularUserRepository repository)
        {
            _repository = repository;
        }
        public async Task ExecuteAsync(UpdateEmailCommand message, CancellationToken token)
        {
            var user = await _repository.GetAsync(message.UserId, token);
            user.ChangeEmail(message.Email);
            await _repository.UpdateAsync(user, token);
        }
    }
}
