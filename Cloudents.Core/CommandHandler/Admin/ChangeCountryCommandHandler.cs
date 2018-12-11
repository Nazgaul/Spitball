using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    public class ChangeCountryCommandHandler : ICommandHandler<ChangeCountryCommand>
    {
        private readonly IRegularUserRepository _userRepository;

        public ChangeCountryCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(ChangeCountryCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.Id, false, token);
            user.Country = message.Country.ToUpperInvariant();
            await _userRepository.UpdateAsync(user, token);

        }
    }
}
