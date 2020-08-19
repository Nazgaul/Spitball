using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Cloudents.Web.Binders
{
    public class HtmlEncodeModelBinder : IModelBinder
    {
        private readonly HtmlEncoder _htmlEncoder;

        public HtmlEncodeModelBinder(HtmlEncoder htmlEncoder)
        {
            _htmlEncoder = htmlEncoder;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            //if (valueProviderResult == ValueProviderResult.None)
            //{
            //    return _fallbackBinder.BindModelAsync(bindingContext);
            //}

            var valueAsString = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(valueAsString))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var result = _htmlEncoder.Encode(valueAsString);
            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}
