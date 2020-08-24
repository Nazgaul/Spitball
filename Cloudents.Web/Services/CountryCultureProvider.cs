using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Cloudents.Web.Services
{
    public class CountryCultureProvider : IRequestCultureProvider
    {
        public async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            try
            {
                var countryService = httpContext.RequestServices.GetService<ICountryService>();

                var country = await countryService.GetUserCountryAsync(httpContext.RequestAborted);

                if (country?.Equals("IL", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return new ProviderCultureResult(new StringSegment("he-IL"));
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}