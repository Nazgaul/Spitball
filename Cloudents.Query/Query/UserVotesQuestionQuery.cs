using System.Collections.Generic;
using Cloudents.Core.DTOs;

namespace Cloudents.Query.Query
{
    public class UserVotesQuestionQuery : IQuery<IEnumerable<UserVoteAnswerDto>>
    {
        public UserVotesQuestionQuery(long userId, long questionId)
        {
            UserId = userId;
            QuestionId = questionId;
        }

        public long UserId { get; }
        public long QuestionId { get; }
    }
}