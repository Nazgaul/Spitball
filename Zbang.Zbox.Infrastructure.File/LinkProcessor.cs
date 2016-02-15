using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.File
{
    public class LinkProcessor : IContentProcessor
    {
        protected readonly IBlobProvider BlobProvider;

        public LinkProcessor(IBlobProvider blobProvider)
        {
            BlobProvider = blobProvider;
        }

        private const string ContentFormat = "<a target=\"_Blank\" href=\"{0}\"><img src=\"{1}\"/></a>";
        public virtual Task<PreviewResult> ConvertFileToWebSitePreview(Uri blobUri, int indexNum, CancellationToken cancelToken = default(CancellationToken))
        {
            if (indexNum > 0)
            {
                return Task.FromResult(new PreviewResult { Content = new List<string>() });
            }
            var previewLink = "https://az779114.vo.msecnd.net/preview/" + WebUtility.UrlEncode(blobUri.AbsoluteUri) +
                              string.Format(".jpg?width={0}&height={1}", 1024, 768);
            return Task.FromResult(new PreviewResult { Content = new List<string>
            {
                string.Format(ContentFormat, blobUri.AbsoluteUri,previewLink)
            } });
            //return Task.FromResult(new PreviewResult { ViewName = "Image", Content = blobsNamesInCache });
        }

        public string TypeOfView
        {
            get { return string.Empty; }
        }


        public virtual bool CanProcessFile(Uri blobName)
        {
            return !blobName.AbsoluteUri.StartsWith(BlobProvider.BlobContainerUrl);
        }


        public virtual async Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            const string url2PngApiKey = "PE733F61DA16EFE";
            const string url2PngPrivateKey = "S_B085D82FEC756";

            string url = WebUtility.UrlEncode(blobUri.AbsoluteUri);

            string getstring = "url=" + url;

            string securityHashUrl2Png = Md5HashPhpCompliant(url2PngPrivateKey + "+" + getstring).ToLower();

            var url2PngLink = "http://api.url2png.com/v6/" + url2PngApiKey + "/" + securityHashUrl2Png + "/" + "png/?" + getstring;

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url2PngLink, cancelToken))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        TraceLog.WriteError("cannot generate link preview " + blobUri.AbsoluteUri);
                        return null;
                    }

                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    using (var stream = new MemoryStream(bytes))
                    {
                        await BlobProvider.UploadFilePreviewAsync(url + ".jpg", stream, "image/jpeg", cancelToken);
                        return null;
                    }
                }
            }

            //BlobProvider.UploadFilePreviewAsync(blobName + ".jpg", msPreview, "image/jpeg");
            //return Task.FromResult<PreProcessFileResult>(null);
        }


        private static string Md5HashPhpCompliant(string pass)
        {

            using (var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {

                byte[] dataMd5 = md5.ComputeHash(Encoding.UTF8.GetBytes(pass));
                var sb = new StringBuilder();

                for (int i = 0; i <= dataMd5.Length - 1; i++)
                {
                    sb.AppendFormat("{0:x2}", dataMd5[i]);
                }

                return sb.ToString();
            }

        }

        public string GetDefaultThumbnailPicture()
        {
            return DefaultPicture.LinkTypePicture;
        }


        public Task<string> ExtractContent(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            return Task.FromResult<string>(null);
        }
    }
}
