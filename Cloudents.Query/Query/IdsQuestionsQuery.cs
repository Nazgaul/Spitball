using Cloudents.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

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
