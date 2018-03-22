using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cloudents.Api.Binders
{
    public class NullableEnumEntityBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var typeOfEnum = Nullable.GetUnderlyingType(bindingContext.ModelType);
            if (typeOfEnum == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var strValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (strValue.FirstValue == null)
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            try
            {
                var result = Enum.Parse(typeOfEnum, strValue.FirstValue, true);
                bindingContext.Result = ModelBindingResult.Success(result);
                return Task.CompletedTask;
            }
            catch (ArgumentException e)
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            //var enumObj = Enum.ToObject(typeOfEnum, strValue.FirstValue);

            //bindingContext.Result = ModelBindingResult.Success(enumObj);
            //return Task.CompletedTask;
        }
    }
}