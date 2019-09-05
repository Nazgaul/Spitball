using Cloudents.Core.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudents.FunctionsV2.Services
{
    public class HostUriService : IHostUriService
    {
        public Uri GetHostUri()
        {
            var hostName2 = Environment.ExpandEnvironmentVariables("%WEBSITE_HOSTNAME%");
            //var hostName2 = string.Format("http://{0}.azurewebsites.net",
            //    Environment.ExpandEnvironmentVariables("%WEBSITE_HOSTNAME%"));
            if (hostName2.Contains("localhost", StringComparison.OrdinalIgnoreCase))
            {
                hostName2 = "spitball-function-dev2.azurewebsites.net";
            }

            var uri = new UriBuilder("https", hostName2.TrimEnd('/'));
            return uri.Uri;

            //return hostName2;
        }
    }

    public interface IHostUriService
    {
        Uri GetHostUri();
    }
}
