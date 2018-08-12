﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cloudents.Web.Binders
{
    public class ReturnUrlEntityBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var url = bindingContext.HttpContext.Request.Headers["Referer"];
            if (Uri.TryCreate(url, UriKind.Absolute, out var referer))
            {
                var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseNullableQuery(referer.Query);

                var returnUrl = queryDictionary["returnUrl"].ToString();

                bindingContext.Result = ModelBindingResult.Success(new ReturnUrlRequest { Url = returnUrl });
                return Task.CompletedTask;


            }
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }
    }
}
