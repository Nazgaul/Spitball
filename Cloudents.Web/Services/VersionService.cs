using Cloudents.Web.Api;
using Microsoft.Extensions.Configuration;

namespace Cloudents.Web.Services
{
    public class VersionService
    {
        private readonly IConfiguration _configuration;

        public VersionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetVersion()
        {
            return _configuration["version"] ?? typeof(HomePageController).Assembly.GetName().Version.ToString(4);
        }
    }
}
