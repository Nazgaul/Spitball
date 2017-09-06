using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class LinkProcessor : IContentProcessor
    {
        protected readonly IBlobProvider BlobProvider;
        protected readonly IBlobProvider2<IPreviewContainer> BlobProviderPreview;
        private readonly ILogger m_Logger;

        public LinkProcessor(IBlobProvider blobProvider, IBlobProvider2<IPreviewContainer> blobProviderPreview, ILogger logger)
        {
            BlobProvider = blobProvider;
            BlobProviderPreview = blobProviderPreview;
            m_Logger = logger;
        }

        private const string ContentFormat = "<a target=\"_Blank\" href=\"{0}\"><img src=\"{1}\"/></a>";
        public virtual Task<PreviewResult> ConvertFileToWebsitePreviewAsync(Uri blobUri, int index, CancellationToken cancelToken = default(CancellationToken))
        {
            if (index > 0)
            {
                return Task.FromResult(new PreviewResult { Content = new List<string>() });
            }
            var previewLink = "https://az779114.vo.msecnd.net/preview/" + WebUtility.UrlEncode(blobUri.AbsoluteUri) +
                              $".jpg?width={800}&height={768}";
            return Task.FromResult(new PreviewResult
            {
                Content = new List<string>
            {
                string.Format(ContentFormat, blobUri.AbsoluteUri,previewLink)
            }
            });

        }



        public virtual bool CanProcessFile(Uri blobName)
        {
            return !blobName.AbsoluteUri.StartsWith(BlobProvider.StorageContainerUrl);
        }


        public virtual async Task<PreProcessFileResult> PreProcessFileAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            const string url2PngApiKey = "PE733F61DA16EFE";
            const string url2PngPrivateKey = "S_B085D82FEC756";

            var url = WebUtility.UrlEncode(blobUri.AbsoluteUri);
            if (url != null && url.Length > 260) //The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters.
            {
                return null;
            }
            if (await BlobProviderPreview.ExistsAsync(url + ".jpg", cancelToken).ConfigureAwait(false))
            {
                return null;
            }
            var getString = "url=" + url;

            var securityHashUrl2Png = Md5HashPhpCompliant(url2PngPrivateKey + "+" + getString).ToLower();

            var url2PngLink = "http://api.url2png.com/v6/" + url2PngApiKey + "/" + securityHashUrl2Png + "/" + "png/?" + getString;

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url2PngLink, cancelToken).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        m_Logger.Error("cannot generate link preview " + blobUri.AbsoluteUri);
                    }

                    var bytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                    using (var stream = new MemoryStream(bytes))
                    {
                        await BlobProviderPreview.UploadStreamAsync(url + ".jpg", stream, "image/jpeg", cancelToken).ConfigureAwait(false);
                    }
                }
            }
            return null;
        }


        private static string Md5HashPhpCompliant(string pass)
        {

            using (var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {

                var dataMd5 = md5.ComputeHash(Encoding.UTF8.GetBytes(pass));
                var sb = new StringBuilder();

                for (int i = 0; i <= dataMd5.Length - 1; i++)
                {
                    sb.AppendFormat("{0:x2}", dataMd5[i]);
                }

                return sb.ToString();
            }

        }




        public Task<string> ExtractContentAsync(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            return Extensions.TaskExtensions.CompletedTaskString;
        }

    }
}
