using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Item.Commands.FlagItem
{
    public class FlagQuestionCommandHandler :  ICommandHandler<FlagQuestionCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Question> _repository;

        public FlagQuestionCommandHandler(IRepository<RegularUser> userRepository,
            IRepository<Question> questionRepository)
        {
            _userRepository = userRepository;
            _repository = questionRepository;
        }
        public async Task ExecuteAsync(FlagQuestionCommand message, CancellationToken token)
        {
            var answer = await _repository.LoadAsync(message.Id, token);
            User user = null;
            if (message.UserId.HasValue)
            {
                user = await _userRepository.LoadAsync(message.UserId, token);
                Validate(answer, user);
            }

            answer.Flag(message.FlagReason, user);
            await _repository.UpdateAsync(answer, token);
        }

        private void Validate(Question question, User user)
        {
            if (question.User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot flag your own document");
            }
        }
    }
}
