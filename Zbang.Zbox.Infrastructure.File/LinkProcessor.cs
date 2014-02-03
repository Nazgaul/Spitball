using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public class LinkProcessor : IContentProcessor
    {
        private static readonly string ContentFormat = "<iframe class=\"iframeContent\" src=\"{0}\"></iframe>";
        public Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum)
        {
            if (blobUri.Scheme == "http")
            {
                return Task.FromResult<PreviewResult>(new PreviewResult() { ViewName = "LinkDenied", Content = new List<string> { blobUri.AbsoluteUri } });
            }
            return Task.FromResult<PreviewResult>(new PreviewResult { Content = new List<string> { string.Format(ContentFormat, blobUri.AbsoluteUri) } });
        }

        public string TypeOfView
        {
            get { return string.Empty; }
        }


        public bool CanProcessFile(Uri blobName)
        {

            if (blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl))
            {
                return false;
            }
            return true;
        }


        public Task<PreProcessFileResult> PreProcessFile(Uri blobUri)
        {
            return null;
        }

        public string GetDefaultThumbnailPicture()
        {
            return Zbang.Zbox.Infrastructure.Thumbnail.ThumbnailProvider.LinkTypePicture;
        }
    }
}
