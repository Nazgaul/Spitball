using System;
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
        public override Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            //return Task.FromResult(new PreProcessFileResult { ThumbnailName = GetDefaultThumbnailPicture() });
            return PreProcessFileResult.GetEmptyResult;
        }

        //public override string GetDefaultThumbnailPicture()
        //{
        //    return DefaultPicture.SoundFileTypePicture;
        //}

        public override Task<PreviewResult> ConvertFileToWebSitePreviewAsync(Uri blobUri, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var url = BlobProvider.GenerateSharedAccessReadPermissionInStorage(blobUri, 600);
            return Task.FromResult(new PreviewResult { Content = new List<string> { string.Format(ContentFormat, url) } });
        }

        public static readonly string[] AudioExtensions = { ".mp3" };

        public override bool CanProcessFile(Uri blobName)
        {
            return blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl) && AudioExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        }

        public override Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }
    }
}
