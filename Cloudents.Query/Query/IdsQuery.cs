using System.Collections.Generic;
using Cloudents.Core.DTOs;

namespace Cloudents.Query.Query
{
    public class IdsQuery<T> :  
        IQuery<IEnumerable<QuestionFeedDto>>,
        IQuery<IList<DocumentFeedDto>>
    {
        public IdsQuery(IEnumerable<T> ids)
        {
            QuestionIds = ids;
        }

        public IEnumerable<T> QuestionIds { get;  }
    }
}