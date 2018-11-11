using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Cloudents.Web.Identity;
using Microsoft.AspNetCore.Http;

namespace Cloudents.Web.Services
{
    public class CountryProvider : ICountryProvider
    {
        private readonly IIpToLocation _ipToLocation;
        private readonly IHttpContextAccessor _httpContext;

        public CountryProvider(IIpToLocation ipToLocation, IHttpContextAccessor httpContext)
        {
            _ipToLocation = ipToLocation;
            _httpContext = httpContext;
        }

        public async Task<string> GetUserCountryAsync(CancellationToken token)
        {
            var cookieValue = _httpContext.HttpContext.User.Claims.FirstOrDefault(f =>
                string.Equals(f.Type, AppClaimsPrincipalFactory.Country.ToString(),
                    StringComparison.OrdinalIgnoreCase))?.Value;
            if (cookieValue == null)
            {
                cookieValue = _httpContext.HttpContext.Request.Query["country"].FirstOrDefault();
            }
            if (cookieValue == null)
            {

                cookieValue = _httpContext.HttpContext.Request.Cookies["country"];
            }

            if (cookieValue == null)
            {

                var result = await _ipToLocation.GetAsync(_httpContext.HttpContext.Connection.GetIpAddress(),
                    token);
                cookieValue = result?.Address?.CountryCode;


                if (cookieValue == null)
                {
                    
                    return null;
                }
                _httpContext.HttpContext.Response.Cookies.Append("country", cookieValue);
            }
            return cookieValue;
        }


    }

    public interface ICountryProvider
    {
        Task<string> GetUserCountryAsync(CancellationToken token);
    }
}
