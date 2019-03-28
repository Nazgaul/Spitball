using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class UpdateUserSettingsCommandHandler: ICommandHandler<UpdateUserSettingsCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;

        public UpdateUserSettingsCommandHandler(IRepository<RegularUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UpdateUserSettingsCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.ChangeName(message.FirstName, message.LastName);
            user.Description = message.Description;
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
