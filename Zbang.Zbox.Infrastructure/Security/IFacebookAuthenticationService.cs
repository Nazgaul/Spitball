
using System.Threading.Tasks;
namespace Zbang.Zbox.Infrastructure.Security
{
    public interface IFacebookAuthenticationService
    {
        Task<FacebookUserData> FacebookLogIn(string token);
        string GetFacebookUserImage(long facebookId, FacebookPictureType type);
       // Task<FacebookUserData> FacebookLogIn(string token);
    }
}
