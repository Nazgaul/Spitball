﻿using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class UpdateUserImageCommandHandler : ICommandHandler<UpdateUserImageCommand>
    {
        private readonly IRegularUserRepository _userRepository;

        public UpdateUserImageCommandHandler(IRegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(UpdateUserImageCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId,token);
            user.Image = message.ImagePath;
            await _userRepository.UpdateAsync(user, token);
        }
    }
}