using System.Net;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Extensions
{
    internal static class ConnectionInfoExtension
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

        internal static IPAddress GetIpAddress(this ConnectionInfo connection)
        {
            var ip = connection.RemoteIpAddress;
            if (connection.IsLocal())
            {
                return IPAddress.Parse("31.154.39.170");
            }
            return ip.MapToIPv4();
        }
    }
}
