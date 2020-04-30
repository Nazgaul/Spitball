using Cloudents.Web.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;

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

            var result = await _ipToLocation.GetAsync(bindingContext.HttpContext.GetIpAddress(),
                bindingContext.HttpContext.RequestAborted);

            //var result = await _ipToLocation.GetUserCountryAsync(bindingContext.HttpContext.RequestAborted);
            if (result == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }
            bindingContext.Result = ModelBindingResult.Success(result.CountryCode);

        }
    }
}
