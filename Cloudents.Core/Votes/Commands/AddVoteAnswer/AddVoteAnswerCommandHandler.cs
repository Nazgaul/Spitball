using System;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Votes.Commands.AddVoteAnswer
{
    public class AddVoteAnswerCommandHandler : ICommandHandler<AddVoteAnswerCommand>
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Answer> _answerRepository;

        public AddVoteAnswerCommandHandler(IVoteRepository voteRepository,
            IRepository<RegularUser> userRepository, IRepository<Answer> answerRepository)
        {
            _voteRepository = voteRepository;
            _userRepository = userRepository;
            _answerRepository = answerRepository;
        }

        public async Task ExecuteAsync(AddVoteAnswerCommand message, CancellationToken token)
        {
            var vote = await _voteRepository.GetVoteAnswerAsync(message.UserId, message.AnswerId, token);
            if (vote == null)
            {
                var user = await _userRepository.LoadAsync(message.UserId, token);
                var answer = await _answerRepository.LoadAsync(message.AnswerId, token);
                vote = new Vote(user, answer, message.VoteType);
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