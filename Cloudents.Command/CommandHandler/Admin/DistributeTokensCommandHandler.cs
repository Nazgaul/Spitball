﻿using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    [AdminCommandHandler]
    public class DistributeTokensCommandHandler : ICommandHandler<DistributeTokensCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;

        public DistributeTokensCommandHandler(IRepository<RegularUser> userRepository
            )
        {
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(DistributeTokensCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.AwardMoney(message.Price);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
