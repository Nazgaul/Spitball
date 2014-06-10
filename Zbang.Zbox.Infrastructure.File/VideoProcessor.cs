using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.MediaServices;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public class VideoProcessor : FileProcessor
    {
        private const string ContentFormat = "<video class=\"videoframe\" width=\"800\" controls src=\"{0}\"></video>";

        private readonly Lazy<IMediaSevicesProvider> m_MediaServiceProvider;

        public VideoProcessor(IBlobProvider blobProvider, Lazy<IMediaSevicesProvider> mediaServiceProvider)
            : base(blobProvider)
        {
            m_MediaServiceProvider = mediaServiceProvider;
        }

        public override async Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum)
        {
            var blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            var metaData = await m_BlobProvider.FetechBlobMetaDataAsync(blobName);
            string value;
            if (!metaData.TryGetValue(MetaDataConsts.VideoStatus, out value))
            {
                return new PreviewResult { ViewName = "MediaLoading"  };
                //return new PreviewResult(ContentNotReady);
            }
            var url = m_BlobProvider.GenerateSharedAccressReadPermissionInStorage(blobUri, 600);
            return new PreviewResult(string.Format(ContentFormat, url));

        }

        public override async Task<PreProcessFileResult> PreProcessFile(Uri blobUri)
        {
            var newBlobName = await m_MediaServiceProvider.Value.EncodeVideo(blobUri);
            var metaData = new Dictionary<string, string> { { MetaDataConsts.VideoStatus, "done" } };
            await m_BlobProvider.SaveMetaDataToBlobAsync(newBlobName, metaData);
            return new PreProcessFileResult { BlobName = newBlobName, ThumbnailName = GetDefaultThumbnailPicture() };


        }

        public readonly string[] VideoExtenstions = { ".3gp", ".3g2", ".3gp2", ".asf", ".mts", ".m2ts", ".mod", ".dv", ".ts", ".vob", ".xesc", ".mp4", ".mpeg", ".mpg", ".m2v", ".ismv", ".wmv" };


        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(m_BlobProvider.BlobContainerUrl))
            {
                return VideoExtenstions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;
        }

        public override string GetDefaultThumbnailPicture()
        {
            return Thumbnail.ThumbnailProvider.VideoFileTypePicture;
        }
    }
}
