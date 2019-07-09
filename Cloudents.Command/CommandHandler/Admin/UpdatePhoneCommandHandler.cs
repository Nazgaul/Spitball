using Cloudents.Command.Command;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
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
            user.PhoneNumber = message.NewPhone;
            await _repository.UpdateAsync(user, token);
        }
    }
}
