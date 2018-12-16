using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Domain.Entities;
using Cloudents.Domain.Enums;

namespace Cloudents.Core.Answers.Commands.FlagAnswer
{
    public class FlagAnswerCommandHandler : ICommandHandler<FlagAnswerCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Answer> _answerRepository;

        public FlagAnswerCommandHandler(IRepository<RegularUser> userRepository, IRepository<Answer> answerRepository)
        {
            _userRepository = userRepository;
            _answerRepository = answerRepository;
        }
        public async Task ExecuteAsync(FlagAnswerCommand message, CancellationToken token)
        {
            if (!ItemComponent.ValidateFlagReason(message.FlagReason))
            {
                throw new ArgumentException("reason is too long");
            }
                var user = await _userRepository.LoadAsync(message.UserId, token);
            if (!Privileges.CanFlag(user.Score))
            {
                throw new UnauthorizedAccessException("not enough score");
            }
            var answer = await _answerRepository.LoadAsync(message.AnswerId, token);
            
            if (answer.User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot flag your own document");
            }

            answer.Item.State = ItemState.Flagged;
            answer.Item.FlagReason = message.FlagReason;
            await _answerRepository.UpdateAsync(answer, token);
        }
    }
}
