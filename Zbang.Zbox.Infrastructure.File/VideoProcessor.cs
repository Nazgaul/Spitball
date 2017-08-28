using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.MediaServices;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public class VideoProcessor : FileProcessor
    {
        private const string ContentFormat = "<video class=\"videoframe\" width=\"800\" controls src=\"{0}\"></video>";

        private readonly Lazy<IMediaServicesProvider> m_MediaServiceProvider;

        public VideoProcessor(IBlobProvider blobProvider, Lazy<IMediaServicesProvider> mediaServiceProvider)
            : base(blobProvider)
        {
            m_MediaServiceProvider = mediaServiceProvider;
        }

        public override async Task<PreviewResult> ConvertFileToWebsitePreviewAsync(Uri blobUri, int indexNum,
            CancellationToken cancelToken = default(CancellationToken))
        {
            var metaData = await BlobProvider.FetchBlobMetaDataAsync(blobUri, cancelToken).ConfigureAwait(false);
            if (!metaData.TryGetValue(MetadataConst.VideoStatus, out string _))
            {
                return new PreviewResult { ViewName = "MediaLoading" };
                //return new PreviewResult(ContentNotReady);
            }
            var url = BlobProvider.GenerateSharedAccessReadPermissionInStorage(blobUri, 600);
            return new PreviewResult(string.Format(ContentFormat, url));

        }

        public override async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri,
            CancellationToken cancelToken = default(CancellationToken))
        {
            var currentMetaData = await BlobProvider.FetchBlobMetaDataAsync(blobUri, cancelToken).ConfigureAwait(false);
            if (currentMetaData.TryGetValue(MetadataConst.VideoStatus, out string _))
            {
                return null;
            }
            var newBlobName = await m_MediaServiceProvider.Value.EncodeVideoAsync(blobUri, cancelToken)
                .ConfigureAwait(false);
            if (newBlobName == null) throw new ArgumentNullException(nameof(newBlobName));
            var metaData = new Dictionary<string, string> {[MetadataConst.VideoStatus] = "done" };
            await BlobProvider.SaveMetaDataToBlobAsync(newBlobName, metaData, cancelToken).ConfigureAwait(false);
            return new PreProcessFileResult {BlobName = GetBlobNameFromUri(newBlobName)};


        }

        public static readonly string[] VideoExtensions =
        {
            ".3gp", ".3g2", ".3gp2", ".asf", ".mts", ".m2ts", ".mod", ".dv", ".ts", ".vob", ".xesc", ".mp4", ".mpeg",
            ".mpg", ".m2v", ".ismv", ".wmv"
        };


        public override bool CanProcessFile(Uri blobName)
        {
            return blobName.AbsoluteUri.StartsWith(BlobProvider.StorageContainerUrl) &&
                   VideoExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        }

        public override Task<string> ExtractContentAsync(Uri blobUri,
            CancellationToken cancelToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }
    }
}
