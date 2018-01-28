using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Cloudents.Web.Filters
{
    public class LocationEntityBinder : IModelBinder
    {
        private readonly IIpToLocation _ipToLocation;
        private readonly ITempDataDictionaryFactory _tempDataFactory;
        private const string KeyName = "s-l";


        public LocationEntityBinder(IIpToLocation ipToLocation, ITempDataDictionaryFactory tempDataFactory)
        {
            _ipToLocation = ipToLocation;
            _tempDataFactory = tempDataFactory;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var tempData = _tempDataFactory.GetTempData(bindingContext.HttpContext);
            var locationFromClient = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);
            var locationFromTemp = tempData.Get<Location>(KeyName);


            if (locationFromTemp != null)
            {
                bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
                return;
            }
           

            var ipV4 = bindingContext.HttpContext.Connection.GetIpAddress();
            locationFromTemp = await _ipToLocation.GetAsync(ipV4, bindingContext.HttpContext.RequestAborted).ConfigureAwait(false);
            tempData.Put(KeyName, locationFromTemp);
            bindingContext.Result = ModelBindingResult.Success(locationFromTemp);
        }
    }
}
