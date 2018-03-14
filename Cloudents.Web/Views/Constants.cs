using System;
using Microsoft.AspNetCore.Hosting;

namespace Cloudents.Web.Views
{
    public static class Constants
    {
        public  static string GetCdnEndpoint(IHostingEnvironment env)
        {
            if (string.Equals(env.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            if (string.Equals(env.EnvironmentName, "Staging", StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            return "//spitball.azureedge.net";
        }
    }
}