using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class UpdatePhonerCommandHandler : ICommandHandler<UpdatePhonerCommand>
    {
        private readonly IRegularUserRepository _repository;
        public UpdatePhonerCommandHandler(IRegularUserRepository repository)
        {
            _repository = repository;
        }

        public async Task ExecuteAsync(UpdatePhonerCommand message, CancellationToken token)
        {
            var user = await _repository.GetAsync(message.UserId, token);
            user.PhoneNumber = message.NewPhone;
            await _repository.UpdateAsync(user, token);
        }
    }
}
