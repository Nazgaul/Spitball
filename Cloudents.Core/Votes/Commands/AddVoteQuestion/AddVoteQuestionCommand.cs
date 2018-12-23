using Cloudents.Domain.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Votes.Commands.AddVoteQuestion
{
    public class AddVoteQuestionCommand : ICommand
    {
        public AddVoteQuestionCommand(long userId, long questionId, VoteType voteType)
        {
            UserId = userId;
            QuestionId = questionId;
            VoteType = voteType;
        }

        public long UserId { get; }
        public long QuestionId { get; }

        public VoteType VoteType { get; }
    }
}