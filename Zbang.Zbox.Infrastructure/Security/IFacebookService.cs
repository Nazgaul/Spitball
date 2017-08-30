using System.Threading.Tasks;
namespace Zbang.Zbox.Infrastructure.Security
{
    public interface IFacebookService
    {
        Task<FacebookUserData> FacebookLogOnAsync(string token);
        string GetFacebookUserImage(long facebookId, FacebookPictureType type);
    }

    public interface IGoogleService
    {
        Task<GoogleUserData> GoogleLogOnAsync(string token);
    }
}
