
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Zbang.Zbox.Infrastructure.Security
{
    public interface IFacebookService
    {
        Task<FacebookUserData> FacebookLogIn(string token);
        string GetFacebookUserImage(long facebookId, FacebookPictureType type);
       // Task<FacebookUserData> FacebookLogIn(string token);
        Task<IEnumerable<FacebookFriendData>> GetFacebookUserFriends(string authToken);
    }
}
