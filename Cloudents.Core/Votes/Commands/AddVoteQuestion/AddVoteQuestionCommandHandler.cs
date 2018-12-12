using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

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
            var user = await _userRepository.LoadAsync(message.UserId, token);

            if (!Privileges.CanVote(user.Score, message.VoteType))
            {
                throw new UnauthorizedAccessException("not enough score");
            }
            var question = await _questionRepository.LoadAsync(message.QuestionId, token);

            if (question.User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot vote you own document");

            }

            var vote = await _voteRepository.GetVoteQuestionAsync(message.UserId, message.QuestionId, token);
            if (vote == null && message.VoteType == VoteType.None)
            {
                throw new ArgumentException();
            }
            if (vote == null)
            {
                vote = new Vote(user, question, message.VoteType);
                question.Item.VoteCount += (int)vote.VoteType;
                await _voteRepository.AddAsync(vote, token);
                return;
            }

            if (message.VoteType == VoteType.None)
            {
                question.Item.VoteCount -= (int)vote.VoteType;
                await _voteRepository.DeleteAsync(vote, token);
                return;
            }

            question.Item.VoteCount -= (int)vote.VoteType;
            question.Item.VoteCount += (int)message.VoteType;
            vote.VoteType = message.VoteType;

            await _voteRepository.UpdateAsync(vote, token);
        }


    }
}