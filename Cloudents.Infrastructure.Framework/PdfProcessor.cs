using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Cloudents.Core;
using Cloudents.Core.Storage;
using Path = System.IO.Path;

namespace Cloudents.Infrastructure.Framework
{
    public class PdfProcessor : Processor, IPreviewProvider
    {
        private const string CacheVersion = CacheVersionPrefix + "4";

        public PdfProcessor(
            Uri blobUri,
            IBlobProvider blobProvider,
            IBlobProvider<CacheContainer> blobProviderCache)
            : base(blobProvider,  blobProviderCache, blobUri)
        {
            SetLicense();
        }

        private static void SetLicense()
        {
            var license = new License();
            license.SetLicense("Aspose.Total.lic");
        }

        public async Task<IEnumerable<string>> ConvertFileToWebsitePreviewAsync(int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            var blobName = BlobProvider.GetBlobNameFromUri(BlobUri);

            var resolution = new Resolution(150);
            var jpegDevice = new JpegDevice(resolution, 90);
            Stream blobSr = null;

            var pdf = new AsyncLazy<Document>(async () =>
            {
                SetLicense();
                blobSr = await BlobProvider.DownloadFileAsync(BlobUri, cancelToken).ConfigureAwait(false);
                return new Document(blobSr);
            });

            var retVal = await UploadPreviewCacheToAzureAsync(
                indexNum,
                i => CreateCacheFileName(blobName, i),
                async z =>
                {
                    var ms = new MemoryStream();
                    var p = await pdf;
                    jpegDevice.Process(p.Pages[z + 1], ms);
                    return ms;
                }, CacheVersion, "image/jpg", cancelToken).ConfigureAwait(false);
            
            if (pdf._instance.IsValueCreated && blobSr != null)
            {
                blobSr.Dispose();
                pdf._instance.Value.Dispose();
            }
            return retVal;
        }

        protected static string CreateCacheFileName(string blobName, int index)
        {
            return
                $"{Path.GetFileNameWithoutExtension(blobName)}{CacheVersion}_{index}_{Path.GetExtension(blobName)}.jpg";
        }

        public static readonly string[] PdfExtensions = { ".pdf" };

        public static bool CanProcessFile(Uri blobName)
        {
                return PdfExtensions.Contains(Path.GetExtension(blobName.AbsoluteUri).ToLower());
        }

    }
}
