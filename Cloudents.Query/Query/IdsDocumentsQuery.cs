using System.Collections.Generic;
using Cloudents.Core.DTOs;

namespace Cloudents.Query.Query
{
    public class IdsDocumentsQuery<T> : IQuery<IList<DocumentFeedDto>>
    {
        public IdsDocumentsQuery(IEnumerable<T> ids)
        {
            DocumentIds = ids;
        }

        public IEnumerable<T> DocumentIds { get;  }
    }
}