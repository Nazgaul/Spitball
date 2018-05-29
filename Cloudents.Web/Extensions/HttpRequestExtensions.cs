using System.Collections;
using System.Web;
using JetBrains.Annotations;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;

namespace Cloudents.Web.Extensions
{
    public static class HttpRequestExtensions
    {
        [CanBeNull]
        public static HttpBrowserCapabilities GetCapabilities(this HttpRequest request)
        {
            var userAgent = request.Headers["User-Agent"];
            if (!string.IsNullOrEmpty(userAgent))
            {
                return new HttpBrowserCapabilities
                {
                    Capabilities = new Hashtable { { string.Empty, userAgent } }
                };
            }

            return null;
        }
    }
}