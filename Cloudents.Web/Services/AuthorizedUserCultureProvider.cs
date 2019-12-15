using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

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

    public class AuthorizedUserCultureProvider : IRequestCultureProvider
    {
        public async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }
            var userManager = httpContext.RequestServices.GetService<UserManager<User>>();

            var userId = userManager.GetLongUserId(httpContext.User);
            var query = new UserDataByIdQuery(userId);
            var queryBus = httpContext.RequestServices.GetService<IQueryBus>();
            var result = await queryBus.QueryAsync<User>(query, httpContext.RequestAborted);
            if (result.Language == null)
            {
                return null;
            }
            var culture = $"{result.Language.ToString().Split('-')[0]}-{result.Country}";
            httpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return new ProviderCultureResult(new StringSegment(culture));
        }
    }

    public class CountryCultureProvider : IRequestCultureProvider
    {
        public async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var countryService = httpContext.RequestServices.GetService<ICountryService>();

            var country = await countryService.GetUserCountryAsync(httpContext.RequestAborted);

            if (country?.Equals("IL",StringComparison.OrdinalIgnoreCase) == true)
            {
                return new ProviderCultureResult(new StringSegment("he-IL")); 
            }
            return null;
        }
    }
}