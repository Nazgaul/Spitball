using Cloudents.Core.Interfaces;
using Cloudents.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Votes.Commands.AddVoteQuestion
{
    public class AddVoteQuestionCommandHandler : BaseAddVoteCommandHandler<Question, long>, ICommandHandler<AddVoteQuestionCommand>
    {
        //private readonly IVoteRepository _voteRepository;
        //private readonly IRepository<RegularUser> _userRepository;
        //private readonly IRepository<Question> _questionRepository;
        //private readonly IEventStore _eventStore;

        public AddVoteQuestionCommandHandler(IVoteRepository voteRepository,
            IRepository<RegularUser> userRepository,
            IRepository<Question> questionRepository,
            IEventStore eventStore)
            : base(userRepository, voteRepository, questionRepository, eventStore)
        {
        }

        public async Task ExecuteAsync(AddVoteQuestionCommand message, CancellationToken token)
        {
            await BaseExecuteAsync(message.UserId, message.QuestionId, message.VoteType, token);
            //var user = await _userRepository.LoadAsync(message.UserId, token);

            //if (!Privileges.CanVote(user.Score, message.VoteType))
            //{
            //    throw new UnauthorizedAccessException("not enough score");
            //}
            //var question = await _questionRepository.LoadAsync(message.QuestionId, token);

            //if (question.User.Id == user.Id)
            //{
            //    throw new UnauthorizedAccessException("you cannot vote you own document");

            //}

            //var vote = await _voteRepository.GetVoteQuestionAsync(message.UserId, message.QuestionId, token);
            //if (vote == null && message.VoteType == VoteType.None)
            //{
            //    throw new ArgumentException();
            //}
            //if (vote == null)
            //{
            //    vote = new Vote(user, question, message.VoteType);
            //    question.Item.VoteCount += (int)vote.VoteType;
            //    await _voteRepository.AddAsync(vote, token);
            //    return;
            //}

            //if (message.VoteType == VoteType.None)
            //{
            //    question.Item.VoteCount -= (int)vote.VoteType;
            //    await _voteRepository.DeleteAsync(vote, token);

            //    _eventStore.Add(new ItemFlaggedEvent(question));
            //    return;
            //}

            //question.Item.VoteCount -= (int)vote.VoteType;
            //question.Item.VoteCount += (int)message.VoteType;
            //vote.VoteType = message.VoteType;

            //await _voteRepository.UpdateAsync(vote, token);
        }


        protected override Vote CreateVote(RegularUser user, Question question, VoteType vote)
        {
            return new Vote(user, question, vote);
            //throw new NotImplementedException();
        }

        protected override Task ValidateAsync(User user, Question question, CancellationToken token)
        {
            if (question.User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot vote you own document");

            }

            return Task.CompletedTask;
        }

        protected override async Task<Vote> GetVoteAsync(long userId, long id, CancellationToken token)
        {
            return await VoteRepository.GetVoteQuestionAsync(userId, id, token);
        }
    }
}