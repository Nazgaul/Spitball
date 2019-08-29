using Cloudents.Core.DTOs;
using System.Collections.Generic;

namespace Cloudents.Query.Query
{
    public class IdsQuestionsQuery<T> : IQuery<IEnumerable<QuestionFeedDto>>
    {
        public IdsQuestionsQuery(IEnumerable<T> ids)
        {
            QuestionIds = ids;
        }

        public IEnumerable<T> QuestionIds { get; }
    }
}
