
using System.IO;


namespace Zbang.Zbox.Infrastructure.Profile
{
   public interface IProfilePictureProvider
    {
       ProfileImages UploadProfilePictures(Stream profileImage);
       //ProfileImages GetDefaultProfileImage();
       //ProfileImages GetDefaultProfileImage();
    }
}
