using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace Cloudents.Web.Binders
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
            catch (ArgumentException)
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }
        }
    }


    //public class EmailEntityBinder : IModelBinder
    //{
    //    private readonly IMailProvider _mailProvider;

    //    public EmailEntityBinder(IMailProvider mailProvider)
    //    {
    //        _mailProvider = mailProvider;
    //    }

    //    public async Task BindModelAsync(ModelBindingContext bindingContext)
    //    {
    //        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

    //        var t = await _mailProvider.ValidateEmailAsync(valueProviderResult.FirstValue, bindingContext.HttpContext.RequestAborted);
    //        if (t)
    //        {
    //            bindingContext.Result = ModelBindingResult.Success(valueProviderResult.FirstValue);
    //            return;
    //        }

    //        bindingContext.Result = ModelBindingResult.Failed();
    //    }
    //}
}