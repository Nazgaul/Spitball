using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Cloudents.Web.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Binders
{
    public class CountryModelBinder : IModelBinder
    {
        private readonly IIpToLocation _ipToLocation;


        public CountryModelBinder(IIpToLocation ipToLocation)
        {
            _ipToLocation = ipToLocation;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var cookieValue = bindingContext.HttpContext.User.Claims.FirstOrDefault(f =>
                string.Equals(f.Type, AppClaimsPrincipalFactory.Country.ToString(),
                    StringComparison.OrdinalIgnoreCase))?.Value;
            if (cookieValue == null)
            {
                cookieValue = bindingContext.HttpContext.Request.Cookies["country"];
            }

            if (cookieValue == null)
            {

                var result = await _ipToLocation.GetAsync(bindingContext.HttpContext.Connection.GetIpAddress(),
                      bindingContext.HttpContext.RequestAborted);
                cookieValue = result?.Address?.CountryCode;


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
