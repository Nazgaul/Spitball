using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using AppClaimsPrincipalFactory = Cloudents.Web.Identity.AppClaimsPrincipalFactory;
using ILogger = Cloudents.Core.Interfaces.ILogger;

namespace Cloudents.Web.Services
{
    public class CountryService : ICountryService
    {
        private readonly IIpToLocation _ipToLocation;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ICountryProvider _conCountryProvider;
        private readonly ILogger _logger;
        private readonly ConfigurationService _configurationService;
        private const string CookieName = "country";

        public CountryService(IIpToLocation ipToLocation, IHttpContextAccessor httpContext,
            ILogger logger, 
            ConfigurationService configurationService, ICountryProvider conCountryProvider)
        {
            _ipToLocation = ipToLocation;
            _httpContext = httpContext;
            _logger = logger;
            _configurationService = configurationService;
            _conCountryProvider = conCountryProvider;
        }

        public async Task<string> GetUserCountryAsync(CancellationToken token)
        {
            var site = _configurationService.GetSiteName();
            if (site == ConfigurationService.Site.Frymo)
            {
                return Country.India.Name;
            }
            var cookieValue = _httpContext.HttpContext.User.Claims.FirstOrDefault(f =>
                string.Equals(f.Type, AppClaimsPrincipalFactory.Country,
                    StringComparison.OrdinalIgnoreCase))?.Value;
            if (cookieValue != null)
            {
                return cookieValue;
            }

            cookieValue = _httpContext.HttpContext.Request.Query["country"].FirstOrDefault();
            if (cookieValue != null && !Regex.IsMatch(cookieValue, "[A-Za-z]"))
            {
                cookieValue = null;
            }

            if (cookieValue == null)
            {

                cookieValue = _httpContext.HttpContext.Request.Cookies[CookieName];
                if (cookieValue != null)
                {
                    if (_conCountryProvider.ValidateCountryCode(cookieValue))
                    {
                        return cookieValue;
                    }

                    cookieValue = null;
                }
            }

            if (cookieValue == null)
            {
                try
                {

                    var result = await _ipToLocation.GetAsync(_httpContext.HttpContext.GetIpAddress(),
                        token);
                    cookieValue = result?.CountryCode;
                }
                catch (Exception e)
                {
                    _logger.Exception(e, new Dictionary<string, string>()
                    {
                        ["IP"] = _httpContext.HttpContext.GetIpAddress().ToString()
                    });
                }


                if (cookieValue == null)
                {
                    _logger.Error("failed to extract country code");
                    return null;
                }
                
            }
            _httpContext.HttpContext.Response.Cookies.Append(CookieName, cookieValue);
            return cookieValue;
        }


    }



    public interface ICountryService
    {
        Task<string> GetUserCountryAsync(CancellationToken token);
    }
}
