using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class BlobManagement : IJob //: ISchedulerProcess
    {
        private readonly CloudBlobClient m_BlobClient;
        private readonly IContentWriteSearchProvider m_SearchProvider;
        public BlobManagement(IContentWriteSearchProvider searchProvider)
        {
            m_SearchProvider = searchProvider;
            var cloudStorageAccount = CloudStorageAccount.Parse(

                   CloudConfigurationManager.GetSetting("StorageConnectionString"));

            m_BlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }
        public Task RunAsync(CancellationToken cancellationToken)
        {
            var container = m_BlobClient.GetContainerReference("crawl");
            
            return ListBlobsSegmentedInFlatListingAsync(container, cancellationToken);
        }

        public string Name => nameof(BlobManagement);

        public async Task ListBlobsSegmentedInFlatListingAsync(CloudBlobContainer container, CancellationToken cancellationToken)
        {
            var directoryOk = container.GetDirectoryReference("ok");
            var directoryNoUniversity = container.GetDirectoryReference("mapping-university");

            BlobContinuationToken continuationToken = null;
            //Call ListBlobsSegmentedAsync and enumerate the result segment returned, while the continuation token is non-null.
            //When the continuation token is null, the last page has been returned and execution can exit the loop.
            do
            {
                //This overload allows control of the page size. You can return all remaining results by passing null for the maxResults parameter,
                //or by calling a different overload.
                var resultSegment = await container.ListBlobsSegmentedAsync("", true, BlobListingDetails.Metadata, 100, continuationToken, null, null, cancellationToken).ConfigureAwait(false);
                foreach (var blobItem in resultSegment.Results)
                {
                    var blockBlob = blobItem as CloudBlockBlob;
                    if (blockBlob == null)
                    {
                        continue;
                    }
                    var txt = await blockBlob.DownloadTextAsync(cancellationToken).ConfigureAwait(false);
                    var model = JsonConvert.DeserializeObject<CrawlModel>(txt);
                    if (!string.IsNullOrEmpty(model.University))
                    {
                        //var source = directoryNoUniversity.GetBlockBlobReference(model.Id + ".txt");
                        //await source.StartCopyAsync(blockBlob, cancellationToken).ConfigureAwait(false);
                        //while (source.CopyState.Status != CopyStatus.Success)
                        //{
                        //    await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken).ConfigureAwait(false);
                        //}
                        //await blockBlob.DeleteAsync(cancellationToken).ConfigureAwait(false);
                        //continue;
                    }
                    


                }

                //Get the continuation token.
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);
        }


    }
}
