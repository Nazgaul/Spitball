using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Votes.Commands.AddVoteAnswer
{
    public class AddVoteAnswerCommandHandler : ICommandHandler<AddVoteAnswerCommand>
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IAnswerRepository _repository;

        public AddVoteAnswerCommandHandler(IVoteRepository voteRepository,
            IRepository<RegularUser> userRepository,
             IAnswerRepository answerRepository)

        {
            _voteRepository = voteRepository;
            _userRepository = userRepository;
            _repository = answerRepository;
        }

        public async Task ExecuteAsync(AddVoteAnswerCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);

            //if (!Privileges.CanVote(user.Score, type))
            //{
            //    throw new NoEnoughScoreException();
            //}
            var answer = await _repository.LoadAsync(message.AnswerId, token);
            if (answer.State != ItemState.Ok)
            {
                throw new NotFoundException();
            }
            await ValidateAsync(user, answer, token);
            var vote = await GetVoteAsync(user.Id, answer.Id, token);

            if (vote == null && message.VoteType == VoteType.None)
            {
                throw new ArgumentException();
            }
            if (vote == null)
            {
                //vote = new Vote(user, question, message.VoteType);
                vote = CreateVote(user, answer, message.VoteType);
                await _voteRepository.AddAsync(vote, token);

                answer.VoteCount += (int)vote.VoteType;

                if (answer.VoteCount < VotesToFlag)
                {
                    answer.Flag("Too many down vote", null);
                    // _eventStore.Add(new ItemFlaggedEvent(question));
                }
                return;
            }

            if (message.VoteType == VoteType.None)
            {
                answer.VoteCount -= (int)vote.VoteType;
                if (answer.VoteCount < VotesToFlag)
                {
                    answer.Flag("Too many down vote", null);
                    // _eventStore.Add(new ItemFlaggedEvent(question));
                }
                await _voteRepository.DeleteAsync(vote, token);
                return;
            }

            answer.VoteCount -= (int)vote.VoteType;
            answer.VoteCount += (int)message.VoteType;
            if (answer.VoteCount < VotesToFlag)
            {
                answer.Flag("Too many down vote", null);
                // _eventStore.Add(new ItemFlaggedEvent(question));
            }
            vote.VoteType = message.VoteType;

            await _voteRepository.UpdateAsync(vote, token);


        }

        public const int VotesToFlag = -2;

        protected Vote CreateVote(RegularUser user, Answer question, VoteType vote)
        {
            return new Vote(user, question, vote);
        }

        protected Task<Vote> GetVoteAsync(long userId, Guid id, CancellationToken token)
        {
           return _voteRepository.GetVoteAnswerAsync(userId, id, token);
        }

        protected async Task ValidateAsync(User user, Answer answer, CancellationToken token)
        {
            if (answer.User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot vote you own answer");
            }

            var answerExists = await _repository.GetUserAnswerInQuestion(answer.Question.Id, user.Id, token);
            if (answerExists != null)
            {
                throw new UnauthorizedAccessException("you cannot vote if you gave answer");
            }
        }
    }
}