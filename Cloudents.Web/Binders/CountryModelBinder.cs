using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

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
            var cookieValue = bindingContext.HttpContext.Request.Cookies["country"];
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
                bindingContext.HttpContext.Response.Cookies.Append("country", cookieValue, new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = true,
                    Secure = true
                });

            }
            bindingContext.Result = ModelBindingResult.Success(cookieValue);
        }
    }
}
