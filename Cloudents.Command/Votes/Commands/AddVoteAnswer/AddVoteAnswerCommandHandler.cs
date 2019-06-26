using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Votes.Commands.AddVoteAnswer
{
    public class AddVoteAnswerCommandHandler : ICommandHandler<AddVoteAnswerCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IAnswerRepository _repository;

        public AddVoteAnswerCommandHandler(
            IRepository<User> userRepository,
             IAnswerRepository answerRepository)

        {
            _userRepository = userRepository;
            _repository = answerRepository;
        }

        public async Task ExecuteAsync(AddVoteAnswerCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            
            var answer = await _repository.LoadAsync(message.AnswerId, token);
            answer.Vote(message.VoteType,user);
           await _repository.UpdateAsync(answer, token);
          


        }

       
    }
}