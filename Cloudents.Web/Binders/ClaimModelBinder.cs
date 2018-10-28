using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudents.Web.Binders
{
    public class ClaimModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var claim = bindingContext.ModelName;// claimAttr?.Claim;
            if (claim == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;

            }

            var result = bindingContext.HttpContext.User.Claims.FirstOrDefault(f =>
                string.Equals(f.Type, claim.ToString(), StringComparison.OrdinalIgnoreCase));
            if (result == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }
            else
            {
                if (bindingContext.ModelMetadata.ModelType.IsArray)
                {
                    var arr = result.Value.Split(',');
                    //var type = bindingContext.ModelMetadata.ModelType.GetEnumeratedType();
                    //var val = result.Select(s => Convert.ChangeType(s, type));
                    bindingContext.Result = ModelBindingResult.Success(arr);
                    return Task.CompletedTask;
                }

                var nullableType = Nullable.GetUnderlyingType(bindingContext.ModelMetadata.ModelType);
                if (nullableType != null)
                {
                    var val = Convert.ChangeType(result.Value, nullableType);
                    bindingContext.Result = ModelBindingResult.Success(val);
                }
                else
                {
                    var val = Convert.ChangeType(result.Value, bindingContext.ModelMetadata.ModelType);
                    bindingContext.Result = ModelBindingResult.Success(val);
                }
            }

            return Task.CompletedTask;
        }

        

    }
}