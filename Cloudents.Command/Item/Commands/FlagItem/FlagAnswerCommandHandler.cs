using System;
using System.Threading;
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
            User user = null;
            if (message.UserId.HasValue)
            {
                user = await _userRepository.LoadAsync(message.UserId, token);
                if (answer.User.Id == user.Id)
                {
                    throw new UnauthorizedAccessException("you cannot flag your own document");
                }
            }

           
            answer.Flag(message.FlagReason, user);
            await _answerRepository.UpdateAsync(answer, token);
        }
    }
}
