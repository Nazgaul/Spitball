using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;

namespace Cloudents.Query.Documents
{
    public class DocumentAggregateQuery : IQuery<IList<DocumentFeedDto>>
    {
        public DocumentAggregateQuery(int page)
        {
            Page = page;
        }

        public int Page { get; private set; }


        internal sealed class DocumentAggregateQueryHandler : BaseDocumentQueryHandler, IQueryHandler<DocumentAggregateQuery, IList<DocumentFeedDto>>
        {
            public DocumentAggregateQueryHandler(QuerySession session) : base(session)
            {
            }

            protected override bool Filter(Document w)
            {
                throw new System.NotImplementedException();
            }

            public async Task<IList<DocumentFeedDto>> GetAsync(DocumentAggregateQuery query, CancellationToken token)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}