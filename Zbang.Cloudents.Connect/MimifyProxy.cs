using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Zbang.Cloudents.Connect
{
    public class MimifyProxy : IJavaScriptMinifier
    {
        public string Minify(string source)
        {
            var minifier = new Minifier();
            return minifier.MinifyJavaScript(source);
        }
    }

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