using System.Net;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Extensions
{
    public static class ConnectionInfoExtension
    {
        public static bool IsLocal(this ConnectionInfo connection)
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
            var ip = connection.RemoteIpAddress;
            var ipV4 = ip.MapToIPv4();
            if (connection.IsLocal())
            {
                ipV4 = IPAddress.Parse("162.248.69.68");
            }

            return ipV4;
        }
    }
}
