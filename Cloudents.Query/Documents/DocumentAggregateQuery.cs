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


        internal sealed class DocumentAggregateQueryHandler :  IQueryHandler<DocumentAggregateQuery, IList<DocumentFeedDto>>
        {

            private readonly DapperRepository _dapperRepository;

            public DocumentAggregateQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }


            public async Task<IList<DocumentFeedDto>> GetAsync(DocumentAggregateQuery query, CancellationToken token)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}