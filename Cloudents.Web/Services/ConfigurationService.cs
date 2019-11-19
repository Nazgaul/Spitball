using Cloudents.Web.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Net.Http;

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

        public Site GetSiteName()
        {
            Site ParseToEnum(StringValues val)
            {
                if (Enum.TryParse(typeof(Site), val, true, out var site2))
                {
                    return (Site)site2;
                }

                return Site.Spitball;

            }

            if (!_hostingEnvironment.IsProduction())
            {
                var referer = _httpContextAccessor.HttpContext.Request.Headers["referer"].ToString();
                if (!string.IsNullOrEmpty(referer))
                {
                    var siteVal = new Uri(referer).ParseQueryString()["site"];
                    if (siteVal != null)
                    {
                        return ParseToEnum(siteVal);
                    }
                }
                if (_httpContextAccessor.HttpContext.Request.Query.TryGetValue("site", out var val))
                {
                    return ParseToEnum(val);


                }
            }
            return ParseToEnum(_configuration["siteName"] ?? "spitball");

        }

        public enum Site
        {
            Spitball,
            Frymo
        }
    }
}
