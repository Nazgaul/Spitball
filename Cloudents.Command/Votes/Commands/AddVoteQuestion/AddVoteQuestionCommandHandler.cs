using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Votes.Commands.AddVoteQuestion
{
    public class AddVoteQuestionCommandHandler :  ICommandHandler<AddVoteQuestionCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Question> _repository;

        public AddVoteQuestionCommandHandler(
            IRepository<User> userRepository,
            IRepository<Question> questionRepository)

        {
            _userRepository = userRepository;
            _repository = questionRepository;
        }

        public async Task ExecuteAsync(AddVoteQuestionCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
           
            var question = await _repository.LoadAsync(message.QuestionId, token);
            question.Vote(message.VoteType,user);
            await _repository.UpdateAsync(question, token);
        }

    }
}