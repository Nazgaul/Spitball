using Microsoft.AspNet.SignalR;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Cloudents.Connect
{
    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            if (request.User.Identity.IsAuthenticated)
            {
                return request.User.GetUserId().ToString();
            }
            return null;
        }
    }
}