using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Search.Entities;
using Microsoft.Azure.Search;
using Microsoft.Rest.Azure;

namespace Cloudents.Infrastructure.Search
{
    public class DocumentIndexSearch : IDocumentSearch
    {
        private readonly ISearchIndexClient _client;

        public DocumentIndexSearch(SearchServiceClient client)
        {
            _client = client.Indexes.GetClient("item3");
        }

        public async Task<string> ItemContentAsync(long itemId, CancellationToken cancelToken)
        {
            try
            {
                var item =
                    await
                        _client.Documents.GetAsync<Document>
                        (itemId.ToString(CultureInfo.InvariantCulture),
                            new[] { "content" }, cancellationToken: cancelToken).ConfigureAwait(false);
                return item.Content;
            }
            //item may not exists in the search....
            catch (CloudException)
            {
                return null;
            }
        }
    }
}
