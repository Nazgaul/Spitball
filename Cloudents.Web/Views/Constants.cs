using System;
using Microsoft.AspNetCore.Hosting;

namespace Cloudents.Web.Views
{
    public static class Constants
    {
        public  static string GetCdnEndpoint(IHostingEnvironment env)
        {
            if (string.Equals(env.EnvironmentName, "Development", StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Empty;
            }

            if (string.Equals(env.EnvironmentName, "Staging", StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Empty;
            }

            return "//spitball.azureedge.net";
        }
    }
}