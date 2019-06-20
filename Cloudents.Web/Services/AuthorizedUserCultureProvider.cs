using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudents.Web.Services
{
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
            return new ProviderCultureResult(new StringSegment(result.Language.Name));
        }
    }
}