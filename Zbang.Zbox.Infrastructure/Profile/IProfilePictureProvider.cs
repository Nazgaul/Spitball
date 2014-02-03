
using System.IO;
using Zbang.Zbox.Infrastructure.Enums;


namespace Zbang.Zbox.Infrastructure.Profile
{
   public interface IProfilePictureProvider
    {
       ProfileImages UploadProfilePictures(Stream profileImage);
       //ProfileImages GetDefaultProfileImage();
       ProfileImages GetDefaultProfileImage(UserType userType = UserType.Regular);
    }
}
