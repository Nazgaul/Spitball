using System.Collections.Generic;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class IdsQuery<T> :  
        IQuery<IList<QuestionFeedDto>>,
        IQuery<IList<DocumentFeedDto>>
    {
        public IdsQuery(IEnumerable<T> ids)
        {
            QuestionIds = ids;
        }

        public IEnumerable<T> QuestionIds { get;  }
    }
}