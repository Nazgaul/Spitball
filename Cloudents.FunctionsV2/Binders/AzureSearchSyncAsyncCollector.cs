using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.WebJobs;

namespace Cloudents.FunctionsV2.Binders
{
    //public class AzureSearchSyncAsyncCollector : IAsyncCollector<AzureSearchSyncOutput>
    //{
    //    private ISearchIndexClient _indexClient;

    //    public AzureSearchSyncAsyncCollector(ISearchIndexClient indexClient)
    //    {
    //        this._indexClient = indexClient;
    //    }

    //    public Task AddAsync(AzureSearchSyncOutput item, CancellationToken cancellationToken = default(CancellationToken))
    //    {
    //        var d = new Document();
            
    //        IndexBatch.MergeOrUpload()
    //        throw new global::System.NotImplementedException();
    //    }

    //    public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
    //    {
            
    //        throw new global::System.NotImplementedException();
    //    }
    //}
}