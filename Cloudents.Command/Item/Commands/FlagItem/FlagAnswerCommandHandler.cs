﻿using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Item.Commands.FlagItem
{
    public class FlagAnswerCommandHandler :  ICommandHandler<FlagAnswerCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Answer> _answerRepository;

        public FlagAnswerCommandHandler(IRepository<RegularUser> userRepository,
            IRepository<Answer> answerRepository)
        {
            _userRepository = userRepository;
            _answerRepository = answerRepository;
        }

        public async Task ExecuteAsync(FlagAnswerCommand message, CancellationToken token)
        {
            var answer = await _answerRepository.LoadAsync(message.Id, token);
            RegularUser user = await _userRepository.LoadAsync(message.UserId, token);
           
           
            answer.Flag(message.FlagReason, user);
            await _answerRepository.UpdateAsync(answer, token);
        }
    }
}
