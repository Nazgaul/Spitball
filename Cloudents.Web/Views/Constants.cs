using Microsoft.Extensions.Configuration;

namespace Cloudents.Web.Views
{
    public static class Constants
    {
        public static string GetCdnEndpoint(IConfiguration env)
        {
            return env["cdnEndpoint"] ?? string.Empty;
        }
    }
}