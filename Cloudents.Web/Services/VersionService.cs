using Cloudents.Web.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Cloudents.Web.Services
{
    public class ConfigurationService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ConfigurationService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }

        public string GetVersion()
        {
            return _configuration["version"] ?? typeof(HomePageController).Assembly.GetName().Version.ToString(4);
        }

        public string GetSiteName()
        {
            if (!_hostingEnvironment.IsProduction())
            {
                if (_httpContextAccessor.HttpContext.Request.Query.TryGetValue("site", out var val))
                {
                    return val.ToString();
                }
            }
            
            return _configuration["site"] ?? "spitball";
        }
    }
}
