using System.Collections.Generic;
using Cloudents.Application.DTOs;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Query
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