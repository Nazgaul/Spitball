using System;
using System.IO;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.Profile
{
    public class ProfilePictureProvider : IProfilePictureProvider
    {
        //const string FileName = "userpic";
        //const string LibName = "Lib";
        //const int StartIndexOfPicturesUser = 9;
        //const int NumberOfRandomPicturesOfUser = 1;
        //const int NumberOfRandomPicturesOfLib = 1;

        //private static bool _firstTime = true;


        private readonly IBlobProvider m_BlobProvider;
        private readonly IProfileProcessor m_ProfileImageProcessor;
        //private readonly List<ProfileImages> m_DefaultUserProfilePictures = new List<ProfileImages>();

        public ProfilePictureProvider(IBlobProvider blobProvider, IProfileProcessor profileImageProcessor)
        {
            m_BlobProvider = blobProvider;
            m_ProfileImageProcessor = profileImageProcessor;
            //PopulateDefaultImagesUrl();

            //if (_firstTime)
            //{
            //    _firstTime = false;
            //    CreateProfileImages();
            //}
        }
        //public ProfileImages GetDefaultProfileImage()
        //{

        //    return m_DefaultUserProfilePictures[0];

        //}

        public async Task<ProfileImages> UploadProfilePicturesAsync(Stream profileImage)
        {
            // Uri uri, uriLarge;
            // var imgResizer = new ImageResizer();
            var imageName = Guid.NewGuid().ToString();

            using (
                var ms = m_ProfileImageProcessor.ProcessFile(profileImage, (int)ImageSize.S100X100,
                    (int)ImageSize.S100X100))
            {
                var uri = await m_BlobProvider.UploadProfilePictureAsync(CreateBlobFileNameWithFolder(imageName, ImageSize.S100X100), ms);
                return new ProfileImages(uri);
            }

            //using (var ms = imgResizer.ResizeImageAndSave(profileImage, (int)ImageSize.S50X50, (int)ImageSize.S50X50, true))
            //{
            //    uri = m_BlobProvider.UploadProfilePicture(CreateBlobFileNameWithFolder(imageName, ImageSize.S50X50), ms.ToArray());
            //    //uri = m_BlobProvider.UploadProfilePicture(imageName, ms.ToArray(), ImageSize.S50X50);
            //}

            //using (var ms = imgResizer.ResizeImageAndSave(profileImage, (int)ImageSize.S100X100, (int)ImageSize.S100X100, true))
            //{
            //    uriLarge = m_BlobProvider.UploadProfilePicture(CreateBlobFileNameWithFolder(imageName, ImageSize.S100X100), ms.ToArray());
            //    //uriLarge = m_BlobProvider.UploadProfilePicture(imageName, ms.ToArray(), ImageSize.S100X100);
            //}


        }

        //private void PopulateDefaultImagesUrl()
        //{
        //    for (int i = StartIndexOfPicturesUser; i < StartIndexOfPicturesUser + NumberOfRandomPicturesOfUser; i++)
        //    {
        //        var uri = m_BlobProvider.ProfileContainerUrl + CreateBlobFileNameWithFolder(FileName + i, ImageSize.S50X50);
        //        var uriLarge = m_BlobProvider.ProfileContainerUrl + CreateBlobFileNameWithFolder(FileName + i, ImageSize.S100X100);
        //        m_DefaultUserProfilePictures.Add(new ProfileImages(uri, uriLarge));
        //    }
        //}

        //private void CreateProfileImages()
        //{
        //    var assembly = Assembly.GetExecutingAssembly();
        //    for (int i = StartIndexOfPicturesUser; i < StartIndexOfPicturesUser + NumberOfRandomPicturesOfUser; i++)
        //    {
        //        ReadProfileResourceAndUpload(assembly, i, ImageSize.S50X50, FileName);
        //        ReadProfileResourceAndUpload(assembly, i, ImageSize.S100X100, FileName);
        //    }
        //    for (int i = 1; i <= NumberOfRandomPicturesOfLib; i++)
        //    {
        //        ReadProfileResourceAndUpload(assembly, i, ImageSize.S50X50, LibName);
        //        ReadProfileResourceAndUpload(assembly, i, ImageSize.S100X100, LibName);
        //    }
        //}

        //private void ReadProfileResourceAndUpload(Assembly assembly, int i, ImageSize imageSize, string fileName)
        //{
        //    using (var stream = assembly.GetManifestResourceStream(string.Format("Zbang.Zbox.Infrastructure.Profile.Resources.{0}{1}{2}.jpg", imageSize.ToString("G"), fileName, i)))
        //    {
        //        m_BlobProvider.UploadProfilePicture(CreateBlobFileNameWithFolder(fileName + i, imageSize), stream.ConvertToByteArray());
        //    }
        //}

        private string CreateBlobFileNameWithFolder(string fileName, ImageSize imageSize)
        {
            fileName = Path.ChangeExtension(fileName, ".jpg");
            return Path.Combine(imageSize.ToString("G"), fileName).Replace("\\", "/");
        }
    }
}
