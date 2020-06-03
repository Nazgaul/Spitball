using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using Cloudents.Infrastructure;
using Cloudents.Query;
using Cloudents.Web.Extensions;

namespace Cloudents.Web.Binders
{
    public class CountryModelBinder : IModelBinder
    {
        private readonly IQueryBus _ipToLocation;


        public CountryModelBinder(IQueryBus ipToLocation)
        {
            _ipToLocation = ipToLocation;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var query = new CountryByIpQuery(bindingContext.HttpContext.GetIpAddress().ToString());
            var result = await _ipToLocation.QueryAsync(query, bindingContext.HttpContext.RequestAborted);
            if (result == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }
            bindingContext.Result = ModelBindingResult.Success(result.CountryCode);

        }
    }
}
