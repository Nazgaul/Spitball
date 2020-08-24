using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudents.Web.Services
{
    public class FrymoCultureProvider : IRequestCultureProvider
    {
        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var configurationService = httpContext.RequestServices.GetService<ConfigurationService>();
            var site = configurationService.GetSiteName();
            if (site == ConfigurationService.Site.Frymo)
            {
                return Task.FromResult(new ProviderCultureResult("en-IN"));
            }

            return Task.FromResult<ProviderCultureResult>(null);
            //throw new NotImplementedException();
        }
    }
}