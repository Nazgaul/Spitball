using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;

namespace Cloudents.Web.Binders
{
    public class ClaimModelBinder : IModelBinder
    {
        private readonly IStringLocalizer<DataAnnotationSharedResource> _localizer;

        public ClaimModelBinder(IStringLocalizer<DataAnnotationSharedResource> localizer)
        {
            _localizer = localizer;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var holderType = bindingContext.ModelMetadata.ContainerType;
            if (holderType == null)
            {
                return FailedRequest(bindingContext);
            }

            var propertyType = holderType.GetProperty(bindingContext.ModelMetadata.PropertyName);
            var claimAttr = propertyType?.GetCustomAttribute<ClaimModelBinderAttribute>();
            
            var claim = claimAttr?.Claim;
            if (claim == null)
            {
                return FailedRequest(bindingContext);

            }
            else
            {
                var result = bindingContext.HttpContext.User.Claims.FirstOrDefault(f =>
                    string.Equals(f.Type, claim.ToString(), StringComparison.OrdinalIgnoreCase));
                if (result == null)
                {
                    return FailedRequest(bindingContext);

                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Success(result.Value);
                }
            }

            return Task.CompletedTask;
        }

        private Task FailedRequest(ModelBindingContext bindingContext)
        {
            bindingContext.ModelState.TryAddModelError(
                bindingContext.ModelName,
                _localizer["NeedUniversity"]);
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }
    }
}