﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.File
{
    public class AudioProcessor : FileProcessor
    {
        private const string ContentFormat = "<audio controls=\"controls\"><source src=\"{0}\" type=\"audio/mp3\" /></audio>";

        public AudioProcessor(IBlobProvider blobProvider)
            : base(blobProvider)
        {

        }
        public override Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            return Task.FromResult(new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() });
        }

        public override string GetDefaultThumbnailPicture()
        {
            return SoundFileTypePicture;
        }

        public override Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int width, int height, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var url = BlobProvider.GenerateSharedAccressReadPermissionInStorage(blobUri, 600);
            return Task.FromResult(new PreviewResult { Content = new List<string> { string.Format(ContentFormat, url) } });
        }

        public readonly string[] AudioExtensions = { ".mp3" };

        public override bool CanProcessFile(Uri blobName)
        {
            return blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl) && AudioExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        }

        public override Task<string> ExtractContent(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            return null;
        }
    }
}
