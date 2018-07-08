using System.Net;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Extensions.Extensions
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

        public static IPAddress GetIpAddress(this ConnectionInfo connection)
        {
            if (connection.IsLocal())
            {
                return IPAddress.Parse("62.219.65.138");
            }
            var ip = connection.RemoteIpAddress;
            return ip?.MapToIPv4();
        }
    }
}
