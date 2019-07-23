using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class UpdateNameCommandHandler : ICommandHandler<UpdateNameCommand>
    {
        private readonly IRegularUserRepository _repository;
        public UpdateNameCommandHandler(IRegularUserRepository repository)
        {
            _repository = repository;
        }
        public async Task ExecuteAsync(UpdateNameCommand message, CancellationToken token)
        {
            var user = await _repository.GetAsync(message.UserId, token);
            user.ChangeName(message.FirstName, message.LastName);
            await _repository.UpdateAsync(user, token);
        }
    }
}
