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
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class BlobManagement : IJob //: ISchedulerProcess
    {
        private readonly CloudBlobClient m_BlobClient;
        private readonly IContentWriteSearchProvider m_SearchProvider;
        private readonly IWatsonExtract m_WatsonExtractProvider;
        private readonly IUniversityReadSearchProvider m_UniversitySearchProvider;
        private readonly ILogger m_Logger;

        public BlobManagement(IContentWriteSearchProvider searchProvider, IWatsonExtract watsonExtractProvider, IUniversityReadSearchProvider universitySearchProvider, ILogger logger)
        {
            m_SearchProvider = searchProvider;
            m_WatsonExtractProvider = watsonExtractProvider;
            m_UniversitySearchProvider = universitySearchProvider;
            m_Logger = logger;
            var cloudStorageAccount = CloudStorageAccount.Parse(

                   CloudConfigurationManager.GetSetting("StorageConnectionString"));

            m_BlobClient = cloudStorageAccount.CreateCloudBlobClient();
        }
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var container = m_BlobClient.GetContainerReference("crawl");
            var index = RoleIndexProcessor.GetIndex();
            if (index != 0)
            {
                return;
            }
            var directoryOk = container.GetDirectoryReference("ok");
            var directoryMappingUniversity = container.GetDirectoryReference("mapping-university");
            // await DeleteStuffAsync(directoryOk, cancellationToken).ConfigureAwait(false);

            BlobContinuationToken continuationToken = null;
            //Call ListBlobsSegmentedAsync and enumerate the result segment returned, while the continuation token is non-null.
            //When the continuation token is null, the last page has been returned and execution can exit the loop.
            do
            {

                var resultSegment = await container
                    .ListBlobsSegmentedAsync("", false, BlobListingDetails.None, 100, continuationToken, null, null,
                        cancellationToken).ConfigureAwait(false);
                foreach (var blobItem in resultSegment.Results)
                {
                    try
                    {
                        var blockBlob = blobItem as CloudBlockBlob;
                        if (blockBlob == null)
                        {
                            continue;
                        }
                        var txt = await blockBlob.DownloadTextAsync(cancellationToken).ConfigureAwait(false);
                        var model = JsonConvert.DeserializeObject<CrawlModel>(txt);
                        m_Logger.Info($"{Name} processing {model}");
                        var endPoint = new Uri(model.Url);
                        if (endPoint.AbsolutePath.StartsWith("/flashcard"))
                        {
                            continue;
                        }
                        UniversityByPrefixDto university = null;
                        if (!string.IsNullOrEmpty(model.University))
                        {
                            var query = new UniversitySearchQuery(model.University, 1);
                            var result = await m_UniversitySearchProvider
                                .SearchUniversityAsync(query, cancellationToken).ConfigureAwait(false);
                            university = result.FirstOrDefault();

                            if (university == null)
                            {
                                await CopyBlobAsync(cancellationToken, directoryMappingUniversity, model.Id, blockBlob)
                                    .ConfigureAwait(false);
                                continue;
                            }
                            if (!model.University.Equals(university.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                await CopyBlobAsync(cancellationToken, directoryMappingUniversity, model.Id, blockBlob)
                                    .ConfigureAwait(false);
                                continue;
                            }

                        }

                        model.Content = model.Content?.Remove(0, 22).Trim();

                        var language = await m_WatsonExtractProvider.GetLanguageAsync(model.Content, cancellationToken)
                            .ConfigureAwait(false);
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
                            Image = model.Image,
                            University = university?.Name,
                            UniversityId = university?.Id.ToString()
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

                        await CopyBlobAsync(cancellationToken, directoryOk, model.Id, blockBlob).ConfigureAwait(false);

                    }
                    catch (Exception ex)
                    {
                        m_Logger.Exception(ex, new Dictionary<string, string> { { "Name", Name } });
                    }
                }


                //Get the continuation token.
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);

        }

        private static async Task CopyBlobAsync(CancellationToken cancellationToken, CloudBlobDirectory directory, string id,
            CloudBlockBlob blockBlob)
        {
            var source = directory.GetBlockBlobReference(id + ".txt");
            await source.StartCopyAsync(blockBlob, cancellationToken).ConfigureAwait(false);
            while (source.CopyState.Status != CopyStatus.Success)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken).ConfigureAwait(false);
            }
            await blockBlob.DeleteAsync(cancellationToken).ConfigureAwait(false);
        }

        public string Name => nameof(BlobManagement);


        private async Task DeleteStuffAsync(CloudBlobDirectory directory, CancellationToken cancellationToken)
        {
            BlobContinuationToken continuationToken = null;
            do
            {
                var resultSegment = await directory.ListBlobsSegmentedAsync(true, BlobListingDetails.None, 100, continuationToken, new BlobRequestOptions(), new OperationContext(), cancellationToken).ConfigureAwait(false);

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
                    if (model != null)
                    {
                        list.Add(model.Id);
                    }

                }
                await m_SearchProvider.DeleteDataAsync(list, cancellationToken).ConfigureAwait(false);

                continuationToken = resultSegment.ContinuationToken;
            } while (continuationToken != null);

        }

    }
}
