using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public class AudioProcessor : FileProcessor
    {
        private static readonly string ContentFormat = "<audio controls=\"controls\"><source src=\"{0}\" type=\"audio/mp3\" /></audio>";
        public AudioProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }
        public override Task<PreProcessFileResult> PreProcessFile(Uri blobUri)
        {
            throw new NotImplementedException();
        }

        public override string GetDefaultThumbnailPicture()
        {
            return Zbang.Zbox.Infrastructure.Thumbnail.ThumbnailProvider.SoundFileTypePicture;
        }

        public override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum)
        {
            var url = m_BlobProvider.GenerateSharedAccressReadPermissionInStorage(blobUri, 600);
            return Task.FromResult<PreviewResult>(new PreviewResult { Content = new List<string> { string.Format(ContentFormat, url) } });
        }

        public readonly string[] audioExtenstions = { ".mp3" };

        public override bool CanProcessFile(Uri blobName)
        {
            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return audioExtenstions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
            }
            return false;
        }
    }
}
