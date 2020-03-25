using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace Cloudents.Web.Binders
{
    //public class ReturnUrlEntityBinder : RefererHeaderModelBinder
    //{
    //    protected override void BindData(ModelBindingContext bindingContext, StringValues val)
    //    {
    //        bindingContext.Result = ModelBindingResult.Success(new ReturnUrlRequest { Url = val });
    //    }

    //    public ReturnUrlEntityBinder() : base("returnUrl")
    //    {
    //    }
    //}


    public abstract class RefererHeaderModelBinder : IModelBinder
    {
        private readonly string _key;

        protected RefererHeaderModelBinder(string key)
        {
            _key = key;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var url = bindingContext.HttpContext.Request.Headers["Referer"];
            if (Uri.TryCreate(url, UriKind.Absolute, out var referer))
            {
                var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseNullableQuery(referer.Query);
                if (queryDictionary == null)
                {
                    return Task.CompletedTask;
                }
                if (queryDictionary.TryGetValue(_key, out var val))
                {
                    BindData(bindingContext, val);
                }
                return Task.CompletedTask;


            }
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        protected abstract void BindData(ModelBindingContext bindingContext, StringValues val);

        //private static void BindData(ModelBindingContext bindingContext, StringValues val)
        //{
        //    bindingContext.Result = ModelBindingResult.Success(new ReturnUrlRequest {Url = val});
        //}
    }
}
