using System;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class UpdateEmailCommandHandler : ICommandHandler<UpdateEmailCommand>
    {
        private readonly IRegularUserRepository _repository;
        private readonly IMailProvider _mailProvider;
        public UpdateEmailCommandHandler(IRegularUserRepository repository, IMailProvider mailProvider)
        {
            _repository = repository;
            _mailProvider = mailProvider;
        }
        public async Task ExecuteAsync(UpdateEmailCommand message, CancellationToken token)
        {
            var result = await _mailProvider.ValidateEmailAsync(message.Email, token);
            if (!result)
            {
                throw new ArgumentException();
            }
            var user = await _repository.LoadAsync(message.UserId, token);
            user.ChangeEmail(message.Email);
            await _repository.UpdateAsync(user, token);
        }
    }
}
