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
using Zbang.Zbox.Infrastructure.Culture;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class BlobManagement : IJob //: ISchedulerProcess
    {
        private readonly CloudBlobClient m_BlobClient;
        private readonly IContentWriteSearchProvider m_SearchProvider;
        private readonly IWatsonExtract m_WatsonExtractProvider;
        public BlobManagement(IContentWriteSearchProvider searchProvider, IWatsonExtract watsonExtractProvider)
        {
            m_SearchProvider = searchProvider;
            m_WatsonExtractProvider = watsonExtractProvider;
            var cloudStorageAccount = CloudStorageAccount.Parse(

                   CloudConfigurationManager.GetSetting("StorageConnectionString"));

            m_BlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var container = m_BlobClient.GetContainerReference("crawl");

            var directoryOk = container.GetDirectoryReference("ok");
            //var directoryNoUniversity = container.GetDirectoryReference("mapping-university");
            //await DeleteStuffAsync(directoryOk, cancellationToken).ConfigureAwait(false);

            BlobContinuationToken continuationToken = null;
            //Call ListBlobsSegmentedAsync and enumerate the result segment returned, while the continuation token is non-null.
            //When the continuation token is null, the last page has been returned and execution can exit the loop.
            do
            {
                //This overload allows control of the page size. You can return all remaining results by passing null for the maxResults parameter,
                //or by calling a different overload.
                var resultSegment = await container.ListBlobsSegmentedAsync("", false, BlobListingDetails.Metadata, 100, continuationToken, null, null, cancellationToken).ConfigureAwait(false);
                foreach (var blobItem in resultSegment.Results)
                {
                    var blockBlob = blobItem as CloudBlockBlob;
                    if (blockBlob == null)
                    {
                        continue;
                    }
                    var txt = await blockBlob.DownloadTextAsync(cancellationToken).ConfigureAwait(false);
                    var model = JsonConvert.DeserializeObject<CrawlModel>(txt);

                    var endPoint = new Uri(model.Url);
                    if (endPoint.AbsolutePath.StartsWith("/flashcard"))
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(model.University))
                    {
                        continue;
                    }

                    model.Content = model.Content?.Remove(0, 22).Trim();
                   
                    var language = await m_WatsonExtractProvider.GetLanguageAsync(model.Content, cancellationToken).ConfigureAwait(false);
                    var tags = model.Tags?.Length > 0 ? model.Tags : model.MetaKeywords;

                    var document = new Document
                    {
                        Id = model.Id,
                        Course = model.Course,
                        CrawlDate = DateTime.UtcNow,
                        Date = model.UrlDate,
                        Domain = model.Domain,
                        MetaContent = model.MetaDescription,
                        Name = model.Name,
                        Source = model.Url,
                        Tags = tags,
                        Views = model.Views,
                        Image = model.Image
                    };
                    switch (language)
                    {
                        case Language.Undefined:
                            document.Content = model.Content;
                            break;
                        case Language.EnglishUs:
                            document.ContentEn = model.Content;
                            break;
                        case Language.Hebrew:
                            document.ContentHe = model.Content;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    await m_SearchProvider.UpdateDataAsync(document, cancellationToken).ConfigureAwait(false);

                    var source = directoryOk.GetBlockBlobReference(model.Id + ".txt");
                    await source.StartCopyAsync(blockBlob, cancellationToken).ConfigureAwait(false);
                    while (source.CopyState.Status != CopyStatus.Success)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken).ConfigureAwait(false);
                    }
                    await blockBlob.DeleteAsync(cancellationToken).ConfigureAwait(false);



                }

                //Get the continuation token.
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);

        }

        public string Name => nameof(BlobManagement);


        private async Task DeleteStuffAsync(CloudBlobDirectory directory, CancellationToken cancellationToken)
        {
            BlobContinuationToken continuationToken = null;
            do
            {
                var resultSegment = await directory.ListBlobsSegmentedAsync(continuationToken, cancellationToken).ConfigureAwait(false);

                var list = new List<string>();
                foreach (var blobItem in resultSegment.Results)
                {
                    var blockBlob = blobItem as CloudBlockBlob;
                    if (blockBlob == null)
                    {
                        continue;
                    }
                    var txt = await blockBlob.DownloadTextAsync(cancellationToken).ConfigureAwait(false);
                    var model = JsonConvert.DeserializeObject<CrawlModel>(txt);
                    list.Add(model.Id);

                }
                await m_SearchProvider.DeleteDataAsync(list, cancellationToken).ConfigureAwait(false);

                continuationToken = resultSegment.ContinuationToken;
            } while (continuationToken != null);

        }

    }
}
