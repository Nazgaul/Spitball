﻿using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cloudents.Web.Filters
{
    public sealed class ValidateRecaptchaAttribute : TypeFilterAttribute
    {
        public ValidateRecaptchaAttribute() : base(typeof(ValidateRecaptchaImpl))
        {
        }

        private class ValidateRecaptchaImpl : ActionFilterAttribute
        {
            private readonly IConfiguration _configuration;
            private readonly IRestClient _httpClient;
            private readonly IHostingEnvironment _environment;

            public ValidateRecaptchaImpl(IConfiguration configuration, IRestClient httpClient, IHostingEnvironment environment)
            {
                _configuration = configuration;
                _httpClient = httpClient;
                _environment = environment;
            }

            public override async Task OnActionExecutionAsync(ActionExecutingContext context,
                ActionExecutionDelegate next)
            {
                if (_environment.IsDevelopment() &&
                    context.HttpContext.Request.Headers["referer"].ToString().Contains("swagger"))
                {
                    await base.OnActionExecutionAsync(context, next);
                    return;
                }

                string captcha;
                context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(context.HttpContext.Request.Body))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var t = await JToken.ReadFromAsync(jsonTextReader);
                    captcha = t["captcha"].Value<string>();
                }
                var secret = _configuration["GoogleReCaptcha:Secret"];
                var nvc = new NameValueCollection()
                {
                    ["secret"] = secret,
                    ["response"] = captcha
                };

                var result = await _httpClient.GetAsync<RecaptchaResponse>(
                    new Uri("https://www.google.com/recaptcha/api/siteverify"), nvc,
                    context.HttpContext.RequestAborted);

                if (result == null)
                {
                    context.Result = new BadRequestResult();
                    return;
                }

                if (!result.Success)
                {
                    context.ModelState.AddModelError("captcha", "captcha is invalid");
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
                await base.OnActionExecutionAsync(context, next);
            }
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local", Justification = "Json.net need to familiar with this")]
        [UsedImplicitly]
        public class RecaptchaResponse
        {
            public bool Success { get; set; }
        }
    }
}
