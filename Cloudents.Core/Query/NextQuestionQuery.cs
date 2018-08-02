using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class NextQuestionQuery : IQuery<IEnumerable<QuestionDto>>
    {
        public NextQuestionQuery(long questionId, long userId)
        {
            QuestionId = questionId;
            UserId = userId;
        }

        public long QuestionId { get; }
        public long UserId { get; }
    }

    public class FictiveUsersQuestionsWithoutCorrectAnswerQuery : IQuery<IEnumerable<QuestionWithoutCorrectAnswerDto>>
    {

    }
}