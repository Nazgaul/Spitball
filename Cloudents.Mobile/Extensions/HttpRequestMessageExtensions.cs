using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;

namespace Cloudents.Mobile.Extensions
{
    internal static class HttpRequestMessageExtensions
    {
        private static IPAddress Parse(string s)
        {
            if (IPAddress.TryParse(s, out var ip))
            {
                return ip;
            }

            return null;
        }
        public static IPAddress GetClientIp(this HttpRequestMessage request)
        {
            if (request.IsLocal())
            {
                return IPAddress.Parse("86.143.189.86");
            }
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                var r = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                return Parse(r);
            }
            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return Parse(prop.Address);
            }
            if (request.Properties.ContainsKey("MS_OwinContext"))
            {
                dynamic owinContext = request.Properties["MS_OwinContext"];
                if (owinContext != null)
                {
                    var s = owinContext.Request.RemoteIpAddress;
                    return Parse(s);
                }
            }

            return null;
        }
    }
}