using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class BlobManagement : IJob //: ISchedulerProcess
    {
        private readonly CloudBlobClient m_BlobClient;
        public BlobManagement()
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(

                   CloudConfigurationManager.GetSetting("StorageConnectionString"));

            m_BlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }
        public Task RunAsync(CancellationToken cancellationToken)
        {
            var container = m_BlobClient.GetContainerReference(StorageContainerName.AzureCacheContainer.ToLower());
            return ListBlobsSegmentedInFlatListingAsync(container);
        }

        public string Name => nameof(BlobManagement);

        public async Task ListBlobsSegmentedInFlatListingAsync(CloudBlobContainer container)
        {
            BlobContinuationToken continuationToken = null;
            var i = 0;
            //Call ListBlobsSegmentedAsync and enumerate the result segment returned, while the continuation token is non-null.
            //When the continuation token is null, the last page has been returned and execution can exit the loop.
            do
            {
                //This overload allows control of the page size. You can return all remaining results by passing null for the maxResults parameter,
                //or by calling a different overload.
                var resultSegment = await container.ListBlobsSegmentedAsync("423e2e06-7bac-4a12-af2a-beb9d2c3e114", true, BlobListingDetails.Metadata, 100, continuationToken, null, null).ConfigureAwait(false);
                //if (resultSegment.Results.Any()) { Console.WriteLine("Page {0}:", ++i); }
                foreach (var blobItem in resultSegment.Results)
                {
                    i++;
                    if (i == 1000)
                    {
                        break;
                    }
                    var blockBlob = blobItem as CloudBlockBlob;
                    if (blockBlob?.Metadata?.Count > 0)
                    {
                        //yay
                    }
                }

                //Get the continuation token.
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);
        }


    }
}
