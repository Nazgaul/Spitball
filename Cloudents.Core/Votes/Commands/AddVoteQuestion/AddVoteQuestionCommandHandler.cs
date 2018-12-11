using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Votes.Commands.AddVoteQuestion
{
    public class AddVoteQuestionCommandHandler : ICommandHandler<AddVoteQuestionCommand>
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Question> _questionRepository;

        public AddVoteQuestionCommandHandler(IVoteRepository voteRepository, IRepository<RegularUser> userRepository, IRepository<Question> questionRepository)
        {
            _voteRepository = voteRepository;
            _userRepository = userRepository;
            _questionRepository = questionRepository;
        }

        public async Task ExecuteAsync(AddVoteQuestionCommand message, CancellationToken token)
        {
            //var user = await _userRepository.LoadAsync(message.UserId, token);
            //var question = await _questionRepository.LoadAsync(message.QuestionId, token);
            //var vote = new Vote(user,question,message.VoteType);
            //await _voteRepository.AddAsync(vote, token);

            var vote = await _voteRepository.GetVoteQuestionAsync(message.UserId, message.QuestionId, token);
            if (vote == null)
            {
                var user = await _userRepository.LoadAsync(message.UserId, token);
                var question = await _questionRepository.LoadAsync(message.QuestionId, token);
                vote = new Vote(user, question, message.VoteType);
                await _voteRepository.AddAsync(vote, token);
                return;
            }

            if (message.VoteType == VoteType.None)
            {
                await _voteRepository.DeleteAsync(vote, token);
                return;
            }

            vote.VoteType = message.VoteType;

            await _voteRepository.UpdateAsync(vote, token);
        }
    }
}