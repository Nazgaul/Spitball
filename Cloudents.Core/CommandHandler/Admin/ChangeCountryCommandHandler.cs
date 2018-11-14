using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    class ChangeCountryCommandHandler : ICommandHandler<ChangeCountryCommand>
    {
        private readonly IUserRepository _userRepository;

        public ChangeCountryCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(ChangeCountryCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, false, token);
            user.Country = message.Country;
            await _userRepository.UpdateAsync(user, token);

        }
    }
}
