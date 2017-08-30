
using System.IO;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Profile
{
    public interface IProfilePictureProvider
    {
        Task<ProfileImages> UploadProfilePicturesAsync(Stream profileImage);
    }
}
