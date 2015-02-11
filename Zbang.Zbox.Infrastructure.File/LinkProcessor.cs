﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;

namespace Zbang.Zbox.Infrastructure.File
{
    public class LinkProcessor : IContentProcessor
    {
        protected readonly IBlobProvider BlobProvider;

        public LinkProcessor(IBlobProvider blobProvider)
        {
            BlobProvider = blobProvider;
        }

        private const string ContentFormat = "<iframe class=\"iframeContent\" src=\"{0}\"></iframe>";

        public Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            if (blobUri.Scheme == "http")
            {
                return Task.FromResult(new PreviewResult { ViewName = "LinkDenied",
                    Content = new List<string> { blobUri.AbsoluteUri } });
            }
            return Task.FromResult(new PreviewResult { Content = new List<string> { string.Format(ContentFormat, blobUri.AbsoluteUri) } });
        }

        public string TypeOfView
        {
            get { return string.Empty; }
        }


        public bool CanProcessFile(Uri blobName)
        {
            return !blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl);
        }


        public Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            return null;
        }

        public string GetDefaultThumbnailPicture()
        {
            return FileProcessor.LinkTypePicture;
        }
    }
}
