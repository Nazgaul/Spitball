using Microsoft.AspNetCore.Http;
using System.Net;

namespace Cloudents.Web.Extensions
{
    public static class ConnectionInfoExtension
    {
        private static bool IsLocal(this ConnectionInfo connection)
        {
            if (connection.RemoteIpAddress != null)
            {
                return IPAddress.IsLoopback(connection.RemoteIpAddress);
            }
            // for in memory TestServer or when dealing with default connection info
            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
            {
                return true;
            }

            return false;
        }

        public static IPAddress GetIpAddress(this HttpContext connection)
        {
            if (connection.Connection.IsLocal())
            {
                return IPAddress.Parse("31.154.39.170");
            }
            //https://github.com/MicrosoftDocs/azure-docs/blob/master/articles/frontdoor/front-door-http-headers-protocol.md
            var frontDoorHeaderIp = connection.Request.Headers["X-Azure-ClientIP"];
            if (!string.IsNullOrEmpty(frontDoorHeaderIp.ToString()))
            {
                if (IPAddress.TryParse(frontDoorHeaderIp, out var address))
                {
                    return address;
                }
            }
            var ip = connection.Connection.RemoteIpAddress;
            return ip?.MapToIPv4();
        }
    }
}
