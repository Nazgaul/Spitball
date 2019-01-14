using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Votes.Commands.AddVoteAnswer;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Votes.Commands.AddVoteQuestion
{
    public class AddVoteQuestionCommandHandler :  ICommandHandler<AddVoteQuestionCommand>
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Question> _repository;

        public AddVoteQuestionCommandHandler(IVoteRepository voteRepository,
            IRepository<RegularUser> userRepository,
            IRepository<Question> questionRepository)

        {
            _voteRepository = voteRepository;
            _userRepository = userRepository;
            _repository = questionRepository;
        }

        public async Task ExecuteAsync(AddVoteQuestionCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);

            //if (!Privileges.CanVote(user.Score, type))
            //{
            //    throw new NoEnoughScoreException();
            //}
            var question = await _repository.LoadAsync(message.QuestionId, token);
            if (question.State != ItemState.Ok)
            {
                throw new NotFoundException();
            }
            await ValidateAsync(user, question, token);
            var vote = await GetVoteAsync(message.UserId, message.QuestionId, token);

            if (vote == null && message.VoteType == VoteType.None)
            {
                throw new ArgumentException();
            }
            if (vote == null)
            {
                //vote = new Vote(user, question, message.VoteType);
                vote = CreateVote(user, question, message.VoteType);
                await _voteRepository.AddAsync(vote, token);

                question.VoteCount += (int)vote.VoteType;

                if (question.VoteCount < AddVoteAnswerCommandHandler.VotesToFlag)
                {
                    question.Flag("Too many down vote", null);
                    // _eventStore.Add(new ItemFlaggedEvent(question));
                }
                return;
            }

            if (message.VoteType == VoteType.None)
            {
                question.VoteCount -= (int)vote.VoteType;
                if (question.VoteCount < AddVoteAnswerCommandHandler.VotesToFlag)
                {
                    question.Flag("Too many down vote", null);
                    // _eventStore.Add(new ItemFlaggedEvent(question));
                }
                await _voteRepository.DeleteAsync(vote, token);
                return;
            }

            question.VoteCount -= (int)vote.VoteType;
            question.VoteCount += (int)message.VoteType;
            if (question.VoteCount < AddVoteAnswerCommandHandler.VotesToFlag)
            {
                question.Flag("Too many down vote", null);
                // _eventStore.Add(new ItemFlaggedEvent(question));
            }
            vote.VoteType = message.VoteType;

            await _voteRepository.UpdateAsync(vote, token);
        }


        protected  Vote CreateVote(RegularUser user, Question question, VoteType vote)
        {
            return new Vote(user, question, vote);
        }

        protected  Task ValidateAsync(User user, Question question, CancellationToken token)
        {
            if (question.User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot vote you own document");

            }

            return Task.CompletedTask;
        }

        protected  async Task<Vote> GetVoteAsync(long userId, long id, CancellationToken token)
        {
            return await _voteRepository.GetVoteQuestionAsync(userId, id, token);
        }
    }
}