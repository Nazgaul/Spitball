using Cloudents.Web.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace Cloudents.Web.Binders
{
    public class CountryModelBinder : IModelBinder
    {
        private readonly ICountryService _ipToLocation;


        public CountryModelBinder(ICountryService ipToLocation)
        {
            _ipToLocation = ipToLocation;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {

            var result = await _ipToLocation.GetUserCountryAsync(bindingContext.HttpContext.RequestAborted);
            if (result == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }
            bindingContext.Result = ModelBindingResult.Success(result);

        }
    }
}
