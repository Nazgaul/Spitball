using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.MediaServices
{
    public class MediaSevicesProvider : IMediaSevicesProvider
    {
        private readonly CloudMediaContext _context;
        private readonly IBlobProvider m_BlobProvider;
        private readonly ILocalStorageProvider m_LocalProvider;


        private const string AccountName = "zboxmediaservices";
        private const string AccountKey = "S52DSdcZS2Bhgr7Ofg5rGtBKByR4eVZDkpx8fOXmi2I=";
        public MediaSevicesProvider(IBlobProvider blobProvider, ILocalStorageProvider localProvider)
        {
            m_BlobProvider = blobProvider;
            m_LocalProvider = localProvider;
            _context = new CloudMediaContext(AccountName, AccountKey);

        }

        public async Task<string> EncodeVideo(Uri blobUrl)
        {
            var id = await CreateAssetAndUploadSingleFile(blobUrl);
            var streamid = EncodeToHtml5(id);

            var blobName = DownloadEncodedVideoToStorage(blobUrl, streamid);

            IAsset assetOrig = GetAsset(id), assetOut = GetAsset(streamid);
            DeleteLocatorsForAsset(assetOrig);
            DeleteLocatorsForAsset(assetOut);

            DeleteAssetFilesForAsset(assetOrig);
            DeleteAssetFilesForAsset(assetOut);

            // Delete assets
            DeleteAsset(assetOrig);
            DeleteAsset(assetOut);

            // DeleteAccessPolicy(policy.Id);
            return blobName;
        }


        private async Task<string> CreateAssetAndUploadSingleFile(Uri blobUrl)
        {
            //_context.DefaultStorageAccount.Name
            var blobName = blobUrl.Segments[blobUrl.Segments.Length - 1];
            // Create a .NET console app
            // Set the project properties to use the full .NET Framework (not Client Profile)
            // With NuGet Package Manager, install windowsazure.mediaservices
            // add: using Microsoft.WindowsAzure.MediaServices.Client;
            if (blobUrl.Host != "127.0.0.1")
            {
                var uploadAssetAzure = uploadToMeidaServiceFromAzureStorage(blobUrl);
                return uploadAssetAzure.Id;
            }
            var stream = await m_BlobProvider.DownloadFileAsync(blobName);
            var uploadFilePath = m_LocalProvider.SaveFileToStorage(stream, blobName);
            //var uploadFilePath = filePath;

            ////var context = new CloudMediaContext("zboxmediaservices", "S52DSdcZS2Bhgr7Ofg5rGtBKByR4eVZDkpx8fOXmi2I=");
            var uploadAsset = _context.Assets.Create(Path.GetFileNameWithoutExtension(blobName), AssetCreationOptions.None);
            var assetFile = uploadAsset.AssetFiles.Create(Path.GetFileName(blobName));
            assetFile.Upload(uploadFilePath);
            return uploadAsset.Id;

        }

        public string DownloadEncodedVideoToStorage(Uri originalBlob, string encodeAssetId)
        {
            if (originalBlob.Host == "127.0.0.1")
            {
                return DownloadToLocalStorage(originalBlob, encodeAssetId);
            }
            return DownloadToAzureStorage(originalBlob, encodeAssetId);
        }

        private string DownloadToAzureStorage(Uri originalBlob, string streamingAssetId)
        {
            var blobName = originalBlob.Segments[originalBlob.Segments.Length - 1];
            var streamingAsset = _context.Assets.Where(a => a.Id == streamingAssetId).FirstOrDefault();
            var assetFiles = streamingAsset.AssetFiles.ToList();
            var streamingAssetFile = assetFiles.Where(f => f.Name.ToLower().EndsWith(".mp4")).FirstOrDefault();

            var daysForWhichStreamingUrlIsActive = 365;
            var accessPolicy = _context.AccessPolicies.Create(streamingAsset.Name, TimeSpan.FromDays(daysForWhichStreamingUrlIsActive),
                                                     AccessPermissions.Read | AccessPermissions.List);
            var locator = _context.Locators.CreateLocator(LocatorType.Sas, streamingAsset, accessPolicy);
            var containerName = new UriBuilder(locator.Path).Path.Replace("/", string.Empty);

            var fileName = streamingAssetFile.Name;

            var blobClient = CreateBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileName);

            var newBlobName = Path.GetFileNameWithoutExtension(blobName) + ".mp4";
            var fileContainer = blobClient.GetContainerReference(BlobProvider.AzureBlobContainer.ToLower());
            var newBlob = fileContainer.GetBlockBlobReference(newBlobName);

            newBlob.StartCopyFromBlob(blob);

            return newBlobName;
        }

        private string DownloadToLocalStorage(Uri originalBlob, string streamingAssetId)
        {
            var blobName = originalBlob.Segments[originalBlob.Segments.Length - 1];
            var locationToSave = string.Empty;
            using (var ms = new MemoryStream())
            {
                locationToSave = m_LocalProvider.SaveFileToStorage(ms, blobName);

            }
            var streamingAsset = _context.Assets.Where(a => a.Id == streamingAssetId).FirstOrDefault();

            var assetFiles = streamingAsset.AssetFiles.ToList();
            var streamingAssetFile = assetFiles.Where(f => f.Name.ToLower().EndsWith(".mp4")).FirstOrDefault();
            streamingAssetFile.Download(locationToSave);

            var newBlobName = Path.GetFileNameWithoutExtension(blobName) + ".mp4";
            m_BlobProvider.UploadFileAsync(newBlobName, locationToSave, "video/mp4");

            m_BlobProvider.SaveMetaDataToBlobAsync(newBlobName, new Dictionary<string, string> { { MetaDataConsts.VideoStatus, "x" } });
            return newBlobName;

        }



        private string EncodeToHtml5(string encodeAssetId)
        {

            // Create a .NET console app
            // Set the project properties to use the full .NET Framework (not Client Profile)
            // With NuGet Package Manager, install windowsazure.mediaservices
            // add: using Microsoft.WindowsAzure.MediaServices.Client;
            // TODO: Uncomment the following if you are not using the previous snippets
            ////var context = new CloudMediaContext("zboxmediaservices", "S52DSdcZS2Bhgr7Ofg5rGtBKByR4eVZDkpx8fOXmi2I=");
            // TODO: Replace with an IAsset.Id string if you are not using the previous snippets
            //var encodeAssetId = assetId; // "YOUR ASSET ID";
            // Preset reference documentation: http://msdn.microsoft.com/en-us/library/windowsazure/jj129582.aspx
            var encodingPreset = "H264 Broadband 720p";
            var assetToEncode = _context.Assets.Where(a => a.Id == encodeAssetId).FirstOrDefault();
            if (assetToEncode == null)
            {
                throw new ArgumentException("Could not find assetId: " + encodeAssetId);
            }

            IJob job = _context.Jobs.Create("Encoding " + assetToEncode.Name + " to " + encodingPreset);

            IMediaProcessor latestWameMediaProcessor = (from p in _context.MediaProcessors where p.Name == "Windows Azure Media Encoder" select p).ToList().OrderBy(wame => new Version(wame.Version)).LastOrDefault();
            ITask encodeTask = job.Tasks.AddNew("Encoding", latestWameMediaProcessor, encodingPreset, TaskOptions.None);
            encodeTask.InputAssets.Add(assetToEncode);
            encodeTask.OutputAssets.AddNew(assetToEncode.Name + " as " + encodingPreset, AssetCreationOptions.None);

            //job.StateChanged += new EventHandler<JobStateChangedEventArgs>((sender, jsc) => 
            //    Console.WriteLine(string.Format("{0}\n  State: {1}\n  Time: {2}\n\n", ((IJob)sender).Name, jsc.CurrentState, DateTime.UtcNow.ToString(@"yyyy_M_d_hhmmss"))));
            job.Submit();
            job.GetExecutionProgressTask(CancellationToken.None).Wait();

            var preparedAsset = job.OutputMediaAssets.FirstOrDefault();
            return preparedAsset.Id;
        }

        private string GetSmoothStreaming(string streamingAssetId)
        {
            // Create a .NET console app
            // Set the project properties to use the full .NET Framework (not Client Profile)
            // With NuGet Package Manager, install windowsazure.mediaservices
            // add: using Microsoft.WindowsAzure.MediaServices.Client;
            // TODO: Uncomment the following if you are not using the previous snippets
            ////var context = new CloudMediaContext("zboxmediaservices", "S52DSdcZS2Bhgr7Ofg5rGtBKByR4eVZDkpx8fOXmi2I=");
            // TODO: Replace with an IAsset.Id string if you are not using the previous snippets
            // var streamingAssetId = preparedAsset.Id; // "YOUR ASSET ID";
            var daysForWhichStreamingUrlIsActive = 365;
            var streamingAsset = _context.Assets.Where(a => a.Id == streamingAssetId).FirstOrDefault();
            var accessPolicy = _context.AccessPolicies.Create(streamingAsset.Name, TimeSpan.FromDays(daysForWhichStreamingUrlIsActive),
                                                     AccessPermissions.Read | AccessPermissions.List);
            string streamingUrl = string.Empty;
            var assetFiles = streamingAsset.AssetFiles.ToList();

            //this code is for smooth streaming - the html5 browser does not support smooth streaming
            //var streamingAssetFile = assetFiles.Where(f => f.Name.ToLower().EndsWith("m3u8-aapl.ism")).FirstOrDefault();
            //if (streamingAssetFile != null)
            //{
            //    var locator = _context.Locators.CreateLocator(LocatorType.OnDemandOrigin, streamingAsset, accessPolicy);
            //    Uri hlsUri = new Uri(locator.Path + streamingAssetFile.Name + "/manifest(format=m3u8-aapl)");
            //    streamingUrl = hlsUri.ToString();
            //}
            //streamingAssetFile = assetFiles.Where(f => f.Name.ToLower().EndsWith(".ism")).FirstOrDefault();
            //if (string.IsNullOrEmpty(streamingUrl) && streamingAssetFile != null)
            //{
            //    var locator = _context.Locators.CreateLocator(LocatorType.OnDemandOrigin, streamingAsset, accessPolicy);
            //    Uri smoothUri = new Uri(locator.Path + streamingAssetFile.Name + "/manifest");
            //    streamingUrl = smoothUri.ToString();
            //}
            var streamingAssetFile = assetFiles.Where(f => f.Name.ToLower().EndsWith(".mp4")).FirstOrDefault();
            var blobPath = string.Empty;
            if (string.IsNullOrEmpty(streamingUrl) && streamingAssetFile != null)
            {
                var locator = _context.Locators.CreateLocator(LocatorType.Sas, streamingAsset, accessPolicy);
                var mp4Uri = new UriBuilder(locator.Path);
                mp4Uri.Path += "/" + streamingAssetFile.Name;
                blobPath = mp4Uri.Path;
                streamingUrl = mp4Uri.ToString();
            }
            return blobPath;
        }

        IAsset GetAsset(string assetId)
        {
            // Use a LINQ Select query to get an asset.
            var assetInstance =
                from a in _context.Assets
                where a.Id == assetId
                select a;
            // Reference the asset as an IAsset.
            IAsset asset = assetInstance.FirstOrDefault();

            return asset;
        }

        //////////////////////////////////////////////////
        // Delete tasks
        //////////////////////////////////////////////////

        public void DeleteAssetFilesForAsset(IAsset asset)
        {
            foreach (IAssetFile file in asset.AssetFiles)
            {
                //Console.WriteLine("Deleting asset file with id: {0} {1}", file.Id, file.Name);
                file.Delete();
            }
        }

        void DeleteAsset(IAsset asset)
        {
            //Console.WriteLine("Deleting the Asset {0}", asset.Id);
            // delete the asset
            asset.Delete();

            // Verify asset deletion
            //if (GetAsset(asset.Id) == null)
            //Console.WriteLine("Deleted the Asset");

        }

        public void DeleteLocatorsForAsset(IAsset asset)
        {
            string assetId = asset.Id;
            var locators = from a in _context.Locators
                           where a.AssetId == assetId
                           select a;

            foreach (var locator in locators)
            {
                //Console.WriteLine("Deleting locator {0} for asset {1}", locator.Path, assetId);

                locator.Delete();
            }
        }

        void DeleteAccessPolicy(string existingPolicyId)
        {
            // To delete a specific access policy, get a reference to the policy.  
            // based on the policy Id passed to the method.
            var policyInstance =
                 from p in _context.AccessPolicies
                 where p.Id == existingPolicyId
                 select p;
            IAccessPolicy policy = policyInstance.FirstOrDefault();

            // Console.WriteLine("Deleting policy {0}", existingPolicyId);
            policy.Delete();

        }




        private IAsset uploadToMeidaServiceFromAzureStorage(Uri blobUri)
        {

            var cloudBlobClient = CreateBlobClient();
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];

            // Create a new asset.
            IAsset asset = _context.Assets.Create("NewAsset_" + Guid.NewGuid(), AssetCreationOptions.None);
            IAccessPolicy writePolicy = _context.AccessPolicies.Create("writePolicy",
                TimeSpan.FromMinutes(120), AccessPermissions.Write);
            ILocator destinationLocator = _context.Locators.CreateLocator(LocatorType.Sas, asset, writePolicy);

            // Get the asset container URI and copy blobs from mediaContainer to assetContainer.
            Uri uploadUri = new Uri(destinationLocator.Path);
            string assetContainerName = uploadUri.Segments[1];
            CloudBlobContainer assetContainer =
                cloudBlobClient.GetContainerReference(assetContainerName);


            string fileName = HttpUtility.UrlDecode(Path.GetFileName(blobUri.AbsoluteUri));

            var sourceCloudBlob = cloudBlobClient.GetContainerReference(BlobProvider.AzureBlobContainer.ToLower()).GetBlockBlobReference(blobName);
            sourceCloudBlob.FetchAttributes();

            if (sourceCloudBlob.Properties.Length > 0)
            {
                IAssetFile assetFile = asset.AssetFiles.Create(fileName);
                var destinationBlob = assetContainer.GetBlockBlobReference(fileName);

                destinationBlob.DeleteIfExists();
                destinationBlob.StartCopyFromBlob(sourceCloudBlob);

                destinationBlob.FetchAttributes();
                //if (sourceCloudBlob.Properties.Length != destinationBlob.Properties.Length)
                //    Console.WriteLine("Failed to copy");
            }


            destinationLocator.Delete();
            writePolicy.Delete();

            // Refresh the asset.
            asset = _context.Assets.Where(a => a.Id == asset.Id).FirstOrDefault();
            return asset;
            //At this point, you can create a job using your asset.

        }

        private CloudBlobClient CreateBlobClient()
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials("zboxstorage", "HQQ2v9EJ0E+7WpkraKJwGyQ7pZ/yXK6YclCeA3e4bki1GnQoTJSNVXDtBZa/5tuEMgzczqgrH9VztfFaNxyiiw=="), true);
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            return cloudBlobClient;
        }

    }
}
