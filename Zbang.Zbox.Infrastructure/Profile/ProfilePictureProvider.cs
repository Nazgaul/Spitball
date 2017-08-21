using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.Profile
{
    public class ProfilePictureProvider : IProfilePictureProvider
    {
        private readonly IBlobProvider m_BlobProvider;

        public ProfilePictureProvider(IBlobProvider blobProvider)
        {
            m_BlobProvider = blobProvider;
        }

        public async Task<ProfileImages> UploadProfilePicturesAsync(Stream profileImage)
        {
            var imageName = Guid.NewGuid().ToString();

            using (
                var ms = await ProcessFileAsync(profileImage, 300, 300).ConfigureAwait(false))
            {
                var uri = await m_BlobProvider.UploadProfilePictureAsync(CreateBlobFileNameWithFolder(imageName), ms).ConfigureAwait(false);
                return new ProfileImages(uri);
            }
        }

        private async Task<Stream> ProcessFileAsync(Stream stream, int width, int height)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid subscription key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "b044cd956e0f4af481cdc7e4c5077aeb");

            // Request parameters and URI.
            string requestParameters = $"width={width}&height={height}&smartCropping=true";
            string uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/generateThumbnail?" + requestParameters;

            // Request body. Try this sample with a locally stored JPEG image.
            //byte[] byteData = stream.ConvertToByteArray(); // GetImageAsByteArray(imageFilePath);

            using (var content = new StreamContent(stream))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(uri, content).ConfigureAwait(false);
                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }

            //using (var content = new ByteArrayContent(byteData))
            //{
            //    // This example uses content type "application/octet-stream".
            //    // The other content types you can use are "application/json" and "multipart/form-data".
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            //    var response = await client.PostAsync(uri, content).ConfigureAwait(false);
            //    return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            //}
            
        }

        private static string CreateBlobFileNameWithFolder(string fileName)
        {
            fileName = Path.ChangeExtension(fileName, ".jpg");
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            return Path.Combine("S100X100", fileName).Replace("\\", "/");
        }
    }
}
