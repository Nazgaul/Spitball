using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Cloudents.Web.Services
{
    public class FacebookQueryStringRequestCultureProvider : IRequestCultureProvider
    {
        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var request = httpContext.Request;
            if (request.Query.TryGetValue("fb_locale", out var value))
            {
                var culture = value.ToString().Replace("_", "-");
                return Task.FromResult(new ProviderCultureResult(culture));
            }
            return Task.FromResult<ProviderCultureResult>(null);
        }
    }
}