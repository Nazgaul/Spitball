using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.Storage.Blob;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.MediaServices;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.Azure.MediaServices
{
    public class MediaServicesProvider : IMediaServicesProvider
    {
        private readonly CloudMediaContext m_Context;
        private readonly BlobProvider m_BlobProvider;
        private readonly ILocalStorageProvider m_LocalProvider;


        private const string AccountName = "zboxmediaservices";
        private const string AccountKey = "S52DSdcZS2Bhgr7Ofg5rGtBKByR4eVZDkpx8fOXmi2I=";
        public MediaServicesProvider(IBlobProvider blobProvider, ILocalStorageProvider localProvider)
        {
            m_BlobProvider = blobProvider as BlobProvider;
            m_LocalProvider = localProvider;
            m_Context = new CloudMediaContext(AccountName, AccountKey);

        }

        public async Task<Uri> EncodeVideoAsync(Uri blobUrl, CancellationToken cancelToken)
        {
            var id = await CreateAssetAndUploadSingleFileAsync(blobUrl, cancelToken).ConfigureAwait(false);
            var streamId = await EncodeToHtml5Async(id, cancelToken).ConfigureAwait(false);

            var blobName = await DownloadEncodedVideoToStorageAsync(blobUrl, streamId, cancelToken).ConfigureAwait(false);

            IAsset assetOrig = GetAsset(id), assetOut = GetAsset(streamId);
            var t1 = DeleteLocatorsForAssetAsync(assetOrig);
            var t2 = DeleteLocatorsForAssetAsync(assetOut);

            await Task.WhenAll(t1, t2).ConfigureAwait(false);

            var d1 = DeleteAssetFilesForAssetAsync(assetOrig);
            var d2 = DeleteAssetFilesForAssetAsync(assetOut);

            await Task.WhenAll(d1, d2).ConfigureAwait(false);
            // Delete assets
            var f1 = DeleteAssetAsync(assetOrig);
            var f2 = DeleteAssetAsync(assetOut);

            await Task.WhenAll(f1, f2).ConfigureAwait(false);
            // DeleteAccessPolicy(policy.Id);
            return blobName;
        }

        public async Task DeleteOldAssetsAsync()
        {

            var deletedAssets = m_Context.Assets.Where(s => s.Created < DateTime.UtcNow.AddDays(-3));

            foreach (var deletedAsset in deletedAssets)
            {
                await DeleteLocatorsForAssetAsync(deletedAsset).ConfigureAwait(false);
                await DeleteAssetFilesForAssetAsync(deletedAsset).ConfigureAwait(false);
                await DeleteAssetAsync(deletedAsset).ConfigureAwait(false);

                foreach (var locator in deletedAsset.Locators)
                {
                    await DeleteAccessPolicyAsync(locator.AccessPolicyId).ConfigureAwait(false);
                }
            }
        }

        Task DeleteAccessPolicyAsync(string existingPolicyId)
        {
            // To delete a specific access policy, get a reference to the policy.  
            // based on the policy Id passed to the method.
            var policyInstance =
                from p in m_Context.AccessPolicies
                where p.Id == existingPolicyId
                select p;
            var policy = policyInstance.FirstOrDefault();
            if (policy != null)
            {
                return policy.DeleteAsync();
            }
            return Task.CompletedTask;

        }


        private async Task<string> CreateAssetAndUploadSingleFileAsync(Uri blobUrl, CancellationToken cancelToken)
        {
            //_context.DefaultStorageAccount.Name
            var blobName = blobUrl.Segments[blobUrl.Segments.Length - 1];
            if (blobUrl.Host != "127.0.0.1")
            {
                var uploadAssetAzure = await CreateAssetFromExistingBlobsAsync(blobUrl).ConfigureAwait(false);
                return uploadAssetAzure.Id;
            }
            var stream = await m_BlobProvider.DownloadFileAsync(blobUrl, cancelToken).ConfigureAwait(false);
            var uploadFilePath = await m_LocalProvider.SaveFileToStorageAsync(stream, blobName, false).ConfigureAwait(false);

            var uploadAsset = m_Context.Assets.Create(Path.GetFileNameWithoutExtension(blobName), AssetCreationOptions.None);
            var assetFile = uploadAsset.AssetFiles.Create(Path.GetFileName(blobName));
            assetFile.Upload(uploadFilePath);
            return uploadAsset.Id;

        }

        public Task<Uri> DownloadEncodedVideoToStorageAsync(Uri originalBlob, string encodeAssetId, CancellationToken cancelToken)
        {
            if (originalBlob == null) throw new ArgumentNullException(nameof(originalBlob));
            if (originalBlob.Host == "127.0.0.1")
            {
                return DownloadToLocalStorageAsync(originalBlob, encodeAssetId, cancelToken);
            }
            return DownloadToAzureStorageAsync(originalBlob, encodeAssetId, cancelToken);
        }

        private async Task<Uri> DownloadToAzureStorageAsync(Uri originalBlob, string streamingAssetId, CancellationToken cancelToken)
        {
            var blobName = originalBlob.Segments[originalBlob.Segments.Length - 1];

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault - azure media services doesn't support first or default
            var streamingAsset = m_Context.Assets.Where(a => a.Id == streamingAssetId).FirstOrDefault();
            if (streamingAsset != null)
            {
                var assetFiles = streamingAsset.AssetFiles.ToList();
                var streamingAssetFile = assetFiles.FirstOrDefault(f => f.Name.ToLower().EndsWith(".mp4"));

                const int daysForWhichStreamingUrlIsActive = 365;
                var accessPolicy = await m_Context.AccessPolicies.CreateAsync(streamingAsset.Name, TimeSpan.FromDays(daysForWhichStreamingUrlIsActive),
                    AccessPermissions.Read | AccessPermissions.List).ConfigureAwait(false);
                var locator = await m_Context.Locators.CreateLocatorAsync(LocatorType.Sas, streamingAsset, accessPolicy).ConfigureAwait(false);
                var containerName = new UriBuilder(locator.Path).Path.Replace("/", string.Empty);

                if (streamingAssetFile != null)
                {
                    var fileName = streamingAssetFile.Name;

                    //            var blobClient = CreateBlobClient();
                    var container = m_BlobProvider.BlobClient.GetContainerReference(containerName);
                    var blob = container.GetBlockBlobReference(fileName);

                    var newBlobName = Path.GetFileNameWithoutExtension(blobName) + ".mp4";
                    var fileContainer = m_BlobProvider.BlobClient.GetContainerReference(BlobProvider.AzureBlobContainer.ToLower());
                    var newBlob = fileContainer.GetBlockBlobReference(newBlobName);

                    await newBlob.StartCopyAsync(blob, cancelToken).ConfigureAwait(false);

                    return newBlob.Uri;
                }
            }
            return null;
        }

        private async Task<Uri> DownloadToLocalStorageAsync(Uri originalBlob, string streamingAssetId, CancellationToken cancelToken)
        {
            var blobName = originalBlob.Segments[originalBlob.Segments.Length - 1];
            string locationToSave;
            using (var ms = new MemoryStream())
            {
                locationToSave = await m_LocalProvider.SaveFileToStorageAsync(ms, blobName, false).ConfigureAwait(false);

            }
            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault -azure media services doesn't support first or default
            var streamingAsset = m_Context.Assets.Where(a => a.Id == streamingAssetId).FirstOrDefault();

            if (streamingAsset != null)
            {
                var assetFiles = streamingAsset.AssetFiles.ToList();
                var streamingAssetFile = assetFiles.FirstOrDefault(f => f.Name.ToLower().EndsWith(".mp4"));
                streamingAssetFile?.Download(locationToSave);
            }

            var newBlobName = Path.GetFileNameWithoutExtension(blobName) + ".mp4";
            await m_BlobProvider.UploadFileAsync(newBlobName, locationToSave, "video/mp4").ConfigureAwait(false);
            var blob = m_BlobProvider.GetFile(newBlobName);


            await m_BlobProvider.SaveMetaDataToBlobAsync(blob.Uri, new Dictionary<string, string> { { MetadataConst.VideoStatus, "x" } }, cancelToken).ConfigureAwait(false);
            return blob.Uri;

        }



        private async Task<string> EncodeToHtml5Async(string encodeAssetId, CancellationToken cancelToken)
        {

            const string encodingPreset = "H264 Broadband 720p";
            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault - media services doesn't support first or default
            var assetToEncode = m_Context.Assets.Where(a => a.Id == encodeAssetId).FirstOrDefault();
            if (assetToEncode == null)
            {
                throw new ArgumentException("Could not find assetId: " + encodeAssetId);
            }

            var job = m_Context.Jobs.Create("Encoding " + assetToEncode.Name + " to " + encodingPreset);

            var latestNameMediaProcessor = GetLatestMediaProcessorByName("Windows Azure Media Encoder");
            var encodeTask = job.Tasks.AddNew("Encoding", latestNameMediaProcessor, encodingPreset, TaskOptions.None);
            encodeTask.InputAssets.Add(assetToEncode);
            encodeTask.OutputAssets.AddNew(assetToEncode.Name + " as " + encodingPreset, AssetCreationOptions.None);
            await job.SubmitAsync().ConfigureAwait(false);
            await job.GetExecutionProgressTask(cancelToken).ConfigureAwait(false);

            var preparedAsset = job.OutputMediaAssets.FirstOrDefault();
            return preparedAsset?.Id;
        }



        IAsset GetAsset(string assetId)
        {
            var assetInstance =
                from a in m_Context.Assets
                where a.Id == assetId
                select a;
            // Reference the asset as an IAsset.
            var asset = assetInstance.FirstOrDefault();

            return asset;
        }

        //////////////////////////////////////////////////
        // Delete tasks
        //////////////////////////////////////////////////

        public static Task DeleteAssetFilesForAssetAsync(IAsset asset)
        {
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            var list = new List<Task>();
            foreach (var file in asset.AssetFiles)
            {
                //Console.WriteLine("Deleting asset file with id: {0} {1}", file.Id, file.Name);
                list.Add(file.DeleteAsync());
            }
            return Task.WhenAll(list);
        }

        private IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName)
        {
            // The possible strings that can be passed into the 
            // method for the mediaProcessor parameter:
            //   Windows Azure Media Encoder
            //   Windows Azure Media Packager
            //   Windows Azure Media Encryptor
            //   Storage Decryption

            var processor = m_Context.MediaProcessors.Where(p => p.Name == mediaProcessorName).
                ToList().OrderBy(p => new Version(p.Version)).LastOrDefault();

            if (processor == null)
                throw new ArgumentException("Unknown media processor");

            return processor;
        }

        private static Task DeleteAssetAsync(IAsset asset)
        {
            //Console.WriteLine("Deleting the Asset {0}", asset.Id);
            // delete the asset
            return asset.DeleteAsync();

            // Verify asset deletion
            //if (GetAsset(asset.Id) == null)
            //Console.WriteLine("Deleted the Asset");

        }

        public Task DeleteLocatorsForAssetAsync(IAsset asset)
        {
            if (asset == null) throw new ArgumentNullException(nameof(asset));
            var assetId = asset.Id;
            var locators = from a in m_Context.Locators
                           where a.AssetId == assetId
                           select a;

            var list = new List<Task>();
            foreach (var locator in locators)
            {
                //Console.WriteLine("Deleting locator {0} for asset {1}", locator.Path, assetId);

                list.Add(locator.DeleteAsync());
            }
            return Task.WhenAll(list);
        }








        public async Task<IAsset> CreateAssetFromExistingBlobsAsync(Uri blobUri)
        {
            if (blobUri == null) throw new ArgumentNullException(nameof(blobUri));
            // Create a new asset. 
            var asset = m_Context.Assets.Create("NewAsset_" + Guid.NewGuid(), AssetCreationOptions.None);

            var writePolicy = await m_Context.AccessPolicies.CreateAsync("writePolicy",
                TimeSpan.FromHours(24), AccessPermissions.Write).ConfigureAwait(false);
            var destinationLocator = await m_Context.Locators.CreateLocatorAsync(LocatorType.Sas, asset, writePolicy).ConfigureAwait(false);

            var destinationBlobStorage = m_BlobProvider.BlobClient;//  _destinationStorageAccount.CreateCloudBlobClient();

            // Get the asset container URI and Blob copy from mediaContainer to assetContainer. 
            var destinationContainerName = new Uri(destinationLocator.Path).Segments[1];

            var assetContainer =
                destinationBlobStorage.GetContainerReference(destinationContainerName);

            if (await assetContainer.CreateIfNotExistsAsync().ConfigureAwait(false))
            {
                await assetContainer.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                }).ConfigureAwait(false);
            }
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            var sourceBlob = m_BlobProvider.GetFile(blobName);
            asset.AssetFiles.Create(sourceBlob.Name);
            CopyBlob(sourceBlob, assetContainer);

            await destinationLocator.DeleteAsync().ConfigureAwait(false);
            await writePolicy.DeleteAsync().ConfigureAwait(false);


            return asset;
        }

        private static void CopyBlob(ICloudBlob sourceBlob, CloudBlobContainer destinationContainer)
        {
            var signature = sourceBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24)
            });

            var destinationBlob = destinationContainer.GetBlockBlobReference(sourceBlob.Name);

            if (!destinationBlob.Exists())
            {
                var blob = new CloudBlockBlob(new Uri(sourceBlob.Uri.AbsoluteUri + signature));
                destinationBlob.StartCopy(blob);

            }
        }



    }
}
