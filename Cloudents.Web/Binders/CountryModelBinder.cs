using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Web.Identity;

namespace Cloudents.Web.Binders
{
    public class CountryModelBinder : IModelBinder
    {
        private readonly IQueryBus _queryBus;
        private readonly IIpToLocation _ipToLocation;


        public CountryModelBinder(IQueryBus queryBus, IIpToLocation ipToLocation)
        {
            _queryBus = queryBus;
            _ipToLocation = ipToLocation;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {

            var cookieValue = bindingContext.HttpContext.User.Claims.FirstOrDefault(f =>
                string.Equals(f.Type, AppClaimsPrincipalFactory.Country.ToString(),
                    StringComparison.OrdinalIgnoreCase))?.Value;
            if (cookieValue == null)
            {
                cookieValue = bindingContext.HttpContext.Request.Query["country"].FirstOrDefault();
            }
            if (cookieValue == null)
            {

                cookieValue = bindingContext.HttpContext.Request.Cookies["country"];
            }

            if (cookieValue == null)
            {
                var query = new CountryQuery(bindingContext.HttpContext.Connection.GetIpAddress());
                cookieValue = await _queryBus.QueryAsync(query, bindingContext.HttpContext.RequestAborted);
                if (cookieValue == null)
                {
                    var result = await _ipToLocation.GetAsync(bindingContext.HttpContext.Connection.GetIpAddress(),
                          bindingContext.HttpContext.RequestAborted);
                    cookieValue = result?.Address?.CountryCode;
                }

                if (cookieValue == null)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }
            }
            bindingContext.Result = ModelBindingResult.Success(cookieValue);
        }
    }
}
