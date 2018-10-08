using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Binders
{
    public class ClaimModelBinder :  IModelBinder
    {
        private readonly IStringLocalizer<DataAnnotationSharedResource> _localizer;

        public ClaimModelBinder(IStringLocalizer<DataAnnotationSharedResource> localizer)
        {
            _localizer = localizer;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //var holderType = bindingContext.ModelMetadata.ContainerType;
            ////var t = bindingContext.ModelMetadata as DefaultModelMetadata
            //if (holderType == null)
            //{
            //   // bindingContext.ModelState.
            //    bindingContext.Result = ModelBindingResult.Failed();
            //    return Task.CompletedTask;
            //    //return FailedRequest(bindingContext);
            //}

            //var propertyType = holderType.GetProperty(bindingContext.ModelMetadata.PropertyName);
            //var claimAttr = propertyType?.GetCustomAttribute<ClaimModelBinderAttribute>();

            var claim = bindingContext.ModelName;// claimAttr?.Claim;
            if (claim == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
                //return FailedRequest(bindingContext);

            }
            else
            {
                var result = bindingContext.HttpContext.User.Claims.FirstOrDefault(f =>
                    string.Equals(f.Type, claim.ToString(), StringComparison.OrdinalIgnoreCase));
                if (result == null)
                {
                    bindingContext.Result = ModelBindingResult.Failed();

                    //return FailedRequest(bindingContext);

                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Success(result.Value);
                }
            }

            return Task.CompletedTask;
        }

        private Task FailedRequest(ModelBindingContext bindingContext, string errorResource)
        {
            bindingContext.ModelState.TryAddModelError(
                bindingContext.ModelName,
                _localizer[errorResource/*"NeedUniversity"*/]);
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }
    }
}