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
                return Task.FromResult(new PreviewResult
                {
                    ViewName = "LinkDenied",
                    Content = new List<string> { blobUri.AbsoluteUri }
                });
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


        public async Task<PreProcessFileResult> PreProcessFile(Uri blobUri, CancellationToken cancelToken = default(CancellationToken))
        {
            string url2pngAPIKey = "PE733F61DA16EFE";
            string url2pngPrivateKey = "S_B085D82FEC756";

            string url = WebUtility.UrlEncode(blobUri.AbsoluteUri);

            string getstring = "fullpage=true&url=" + url;

            string SecurityHash_url2png = Md5HashPHPCompliant(url2pngPrivateKey + "+" + getstring).ToLower();

            var url2pngLink = "http://api.url2png.com/v6/" + url2pngAPIKey + "/" + SecurityHash_url2png + "/" + "png/?" + getstring;

            using (var httpClient = new HttpClient())
            {
                var bytes = await httpClient.GetByteArrayAsync(url2pngLink);
                using (var stream = new MemoryStream(bytes))
                {
                    await BlobProvider.UploadFilePreviewAsync(url + ".jpg", stream, "image/jpeg", cancelToken);
                    return null;
                }
            }

            //BlobProvider.UploadFilePreviewAsync(blobName + ".jpg", msPreview, "image/jpeg");
            //return Task.FromResult<PreProcessFileResult>(null);
        }


        private static string Md5HashPHPCompliant(string pass)
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
