using System;
using System.IO;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;
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
                var ms = m_ProfileImageProcessor.ProcessFile(profileImage, (int)ImageSize.S100X100,
                    (int)ImageSize.S100X100))
            {
                var uri = await m_BlobProvider.UploadProfilePictureAsync(CreateBlobFileNameWithFolder(imageName, ImageSize.S100X100), ms);
                return new ProfileImages(uri);
            }

        }


        private static string CreateBlobFileNameWithFolder(string fileName, ImageSize imageSize)
        {
            fileName = Path.ChangeExtension(fileName, ".jpg");
            return Path.Combine(imageSize.ToString("G"), fileName).Replace("\\", "/");
        }
    }
}
