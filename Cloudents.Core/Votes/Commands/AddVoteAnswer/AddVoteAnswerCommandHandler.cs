using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Votes.Commands.AddVoteAnswer
{
    public class AddVoteAnswerCommandHandler : ICommandHandler<AddVoteAnswerCommand>
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IAnswerRepository _answerRepository;

        public AddVoteAnswerCommandHandler(IVoteRepository voteRepository,
            IRepository<RegularUser> userRepository, IAnswerRepository answerRepository)
        {
            _voteRepository = voteRepository;
            _userRepository = userRepository;
            _answerRepository = answerRepository;
        }

        public async Task ExecuteAsync(AddVoteAnswerCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            if (!Privileges.CanFlag(user.Score, message.VoteType))
            {
                throw new UnauthorizedAccessException("not enough score");
            }
            var answer = await _answerRepository.LoadAsync(message.AnswerId, token);

            if (answer.User.Id == message.UserId)
            {
                throw new UnauthorizedAccessException("you cannot vote you own answer");
            }

            var answerExists = await _answerRepository.GetUserAnswerInQuestion(answer.Question.Id, user.Id, token);
            if (answerExists != null)
            {
                throw new UnauthorizedAccessException("you cannot vote if you gave answer");

            }

            var vote = await _voteRepository.GetVoteAnswerAsync(message.UserId, message.AnswerId, token);
            if (vote == null && message.VoteType == VoteType.None)
            {
                throw new ArgumentException();
            }
            if (vote == null)
            {
                //TODO : need to check
                //var user = await _userRepository.LoadAsync(message.UserId, token);
                vote = new Vote(user, answer, message.VoteType);
                answer.Item.VoteCount += (int)vote.VoteType;
                await _voteRepository.AddAsync(vote, token);
                return;
            }

            if (message.VoteType == VoteType.None)
            {
                answer.Item.VoteCount -= (int)vote.VoteType;
                await _voteRepository.DeleteAsync(vote, token);
                return;
            }

            answer.Item.VoteCount -= (int)vote.VoteType;
            answer.Item.VoteCount += (int)message.VoteType;
            vote.VoteType = message.VoteType;

            await _voteRepository.UpdateAsync(vote, token);


        }
    }
}