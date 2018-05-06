using Microsoft.AspNetCore.Hosting;

namespace Cloudents.Web.Views
{
    public static class Constants
    {
        public static string GetCdnEndpoint(IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                return string.Empty;
            }

            if (env.IsStaging())
            {
                return string.Empty;
            }
            return "//spitball.azureedge.net";
        }
    }
}