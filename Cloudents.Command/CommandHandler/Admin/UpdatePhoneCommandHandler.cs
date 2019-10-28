using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class UpdatePhoneCommandHandler : ICommandHandler<UpdatePhoneCommand>
    {
        private readonly IRegularUserRepository _repository;
        public UpdatePhoneCommandHandler(IRegularUserRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(UpdatePhoneCommand message, CancellationToken token)
        {
            var user = await _repository.GetAsync(message.UserId, token);
            if (user == null)
            {
                throw new NotFoundException();
            }

            if (string.IsNullOrEmpty(user.Country))
            {
                throw new NullReferenceException();
            }
            user.PhoneNumber = message.NewPhone;
            await _repository.UpdateAsync(user, token);
        }
    }
}
