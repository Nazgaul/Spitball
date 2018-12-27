using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Votes.Commands.AddVoteQuestion
{
    public class AddVoteQuestionCommandHandler : BaseAddVoteCommandHandler<Question, long>, ICommandHandler<AddVoteQuestionCommand>
    {
        //private readonly IVoteRepository _voteRepository;
        //private readonly IRepository<RegularUser> _userRepository;
        //private readonly IRepository<Question> _questionRepository;
        //private readonly IEventStore _eventStore;

        public AddVoteQuestionCommandHandler(IVoteRepository voteRepository,
            IRepository<RegularUser> userRepository,
            IRepository<Question> questionRepository)
            : base(userRepository, voteRepository, questionRepository)
        {
        }

        public async Task ExecuteAsync(AddVoteQuestionCommand message, CancellationToken token)
        {
            await BaseExecuteAsync(message.UserId, message.QuestionId, message.VoteType, token);
        }


        protected override Vote CreateVote(RegularUser user, Question question, VoteType vote)
        {
            return new Vote(user, question, vote);
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