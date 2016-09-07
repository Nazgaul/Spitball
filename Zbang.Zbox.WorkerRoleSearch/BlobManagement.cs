using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Zbang.Zbox.WorkerRoleSearch
{
    class BlobManagement //: ISchedulerProcess
    {


         public async Task ListBlobsSegmentedInFlatListing(CloudBlobContainer container)
        {
            //List blobs to the console window, with paging.
            Console.WriteLine("List blobs in pages:");

            int i = 0;
            BlobContinuationToken continuationToken = null;

             //Call ListBlobsSegmentedAsync and enumerate the result segment returned, while the continuation token is non-null.
            //When the continuation token is null, the last page has been returned and execution can exit the loop.
            do
            {
                //This overload allows control of the page size. You can return all remaining results by passing null for the maxResults parameter,
                //or by calling a different overload.
                var resultSegment = await container.ListBlobsSegmentedAsync("", true, BlobListingDetails.Copy, 10, continuationToken, null, null);
                if (resultSegment.Results.Any()) { Console.WriteLine("Page {0}:", ++i); }
                foreach (var blobItem in resultSegment.Results)
                {
                    Console.WriteLine("\t{0}", blobItem.StorageUri.PrimaryUri);
                }
                Console.WriteLine();

                //Get the continuation token.
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);
        }
    }
}
