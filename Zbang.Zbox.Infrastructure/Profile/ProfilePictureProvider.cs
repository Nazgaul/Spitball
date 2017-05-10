using System;
using System.IO;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.Profile
{
    public class ProfilePictureProvider : IProfilePictureProvider
    {
        private readonly IBlobProvider m_BlobProvider;
        private readonly IProfileProcessor m_ProfileImageProcessor;

        public ProfilePictureProvider(IBlobProvider blobProvider, IProfileProcessor profileImageProcessor)
        {
            m_BlobProvider = blobProvider;
            m_ProfileImageProcessor = profileImageProcessor;

        }


        public async Task<ProfileImages> UploadProfilePicturesAsync(Stream profileImage)
        {
            var imageName = Guid.NewGuid().ToString();

            using (
                var ms = await m_ProfileImageProcessor.ProcessFileAsync(profileImage, 300, 300).ConfigureAwait(false))
            {
                var uri = await m_BlobProvider.UploadProfilePictureAsync(CreateBlobFileNameWithFolder(imageName), ms).ConfigureAwait(false);
                return new ProfileImages(uri);
            }

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
