using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class QuestionDataByIdQuery : IQuery<QuestionDetailDto>

    {
        public QuestionDataByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class NextQuestionQuery : IQuery<IEnumerable<QuestionDto>>
    {
        public NextQuestionQuery(long questionId, long userId)
        {
            QuestionId = questionId;
            UserId = userId;
        }

        public long QuestionId { get; private set; }
        public long UserId { get; private set; }
    }

}