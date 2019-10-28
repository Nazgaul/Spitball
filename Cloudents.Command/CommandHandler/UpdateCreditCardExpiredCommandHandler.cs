using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;

namespace Cloudents.Command.CommandHandler
{
    public class UpdateCreditCardExpiredCommandHandler : ICommandHandler<UpdateCreditCardExpiredCommand>
    {
        private readonly IRegularUserRepository _repo;
        public UpdateCreditCardExpiredCommandHandler(IRegularUserRepository repo)
        {
            _repo = repo;
        }

        public async Task ExecuteAsync(UpdateCreditCardExpiredCommand message, CancellationToken token)
        {
            var users = await _repo.GetExpiredCreditCardsAsync(token);
            foreach (var user in users)
            {
                user.DeleteUserPayment();
            }
        }
    }
}
