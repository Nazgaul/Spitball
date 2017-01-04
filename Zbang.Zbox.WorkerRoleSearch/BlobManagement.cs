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
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var container = m_BlobClient.GetContainerReference(StorageContainerName.AzureBlobContainer);
            await ListBlobsSegmentedInFlatListingAsync(container);
        }
        public async Task ListBlobsSegmentedInFlatListingAsync(CloudBlobContainer container)
        {

#pragma warning disable CS0219 // The variable 'i' is assigned but its value is never used
            int i = 0;
#pragma warning restore CS0219 // The variable 'i' is assigned but its value is never used
            BlobContinuationToken continuationToken = null;

            //Call ListBlobsSegmentedAsync and enumerate the result segment returned, while the continuation token is non-null.
            //When the continuation token is null, the last page has been returned and execution can exit the loop.
            do
            {
                //This overload allows control of the page size. You can return all remaining results by passing null for the maxResults parameter,
                //or by calling a different overload.
                var resultSegment = await container.ListBlobsSegmentedAsync("", true, BlobListingDetails.None, 10, continuationToken, null, null);
                //if (resultSegment.Results.Any()) { Console.WriteLine("Page {0}:", ++i); }
                foreach (var blobItem in resultSegment.Results)
                {

                }

                //Get the continuation token.
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);
        }


    }
}
