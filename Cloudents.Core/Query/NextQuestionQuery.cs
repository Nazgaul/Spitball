using System.Collections.Generic;
using Cloudents.Application.DTOs;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Query
{
    public class NextQuestionQuery : IQuery<IEnumerable<QuestionFeedDto>>
    {
        public NextQuestionQuery(long questionId, long userId)
        {
            QuestionId = questionId;
            UserId = userId;
        }

        public long QuestionId { get; }
        public long UserId { get; }
    }
}