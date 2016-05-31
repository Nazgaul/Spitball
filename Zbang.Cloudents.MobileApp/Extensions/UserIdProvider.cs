using Microsoft.AspNet.SignalR;

namespace Zbang.Cloudents.MobileApp.Extensions
{
    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            if (request.User.Identity.IsAuthenticated)
            {
                return request.User.GetCloudentsUserId().ToString();
            }
            return null;

        }
    }
}