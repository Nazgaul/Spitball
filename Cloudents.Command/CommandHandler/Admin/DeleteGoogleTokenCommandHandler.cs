using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class DeleteGoogleTokenCommandHandler : ICommandHandler<DeleteGoogleTokenCommand>
    {
        private readonly IRepository<GoogleTokens> _repository;

        public DeleteGoogleTokenCommandHandler(IRepository<GoogleTokens> repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(DeleteGoogleTokenCommand message, CancellationToken token)
        {
            var googleToken = await _repository.LoadAsync(message.UserId.ToString(), token);
            await _repository.DeleteAsync(googleToken, token);
        }
    }
}
