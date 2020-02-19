using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Item.Commands.FlagItem
{
    public class FlagQuestionCommandHandler : ICommandHandler<FlagQuestionCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Question> _repository;

        public FlagQuestionCommandHandler(IRepository<User> userRepository,
            IRepository<Question> questionRepository)
        {
            _userRepository = userRepository;
            _repository = questionRepository;
        }
        public async Task ExecuteAsync(FlagQuestionCommand message, CancellationToken token)
        {
            var question = await _repository.LoadAsync(message.Id, token);
            BaseUser user = await _userRepository.LoadAsync(message.UserId, token);

            question.Flag(message.FlagReason, user);
            await _repository.UpdateAsync(question, token);
        }
    }
}
