using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Extension;
using Cloudents.Infrastructure;
using Cloudents.Web.Models;
using Microsoft.Extensions.Hosting;

namespace Cloudents.Web.Filters
{
    public sealed class ValidateRecaptchaAttribute : TypeFilterAttribute
    {
        //private string SecretKey { get; }

        public ValidateRecaptchaAttribute(string secretKey) : base(typeof(ValidateRecaptchaImpl))
        {
            Arguments = new object[] { secretKey };

            //SecretKey = secretKey;
        }



        private sealed class ValidateRecaptchaImpl : ActionFilterAttribute
        {
            private readonly string _secretKey;
            private readonly HttpClient _httpClient;
            private readonly IWebHostEnvironment _environment;

            public ValidateRecaptchaImpl(string secretKey, /*IConfiguration configuration,*/ HttpClient httpClient, IWebHostEnvironment environment)
            {
                _secretKey = secretKey;
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

                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    await base.OnActionExecutionAsync(context, next);
                    return;
                }
                var captcha = ScanObject(context);

                if (string.IsNullOrEmpty(captcha))
                {
                    context.Result = new BadRequestResult();
                    return;
                }
                // var secret = _configuration["GoogleReCaptcha:Secret"];
                var nvc = new NameValueCollection()
                {
                    ["secret"] = _secretKey,
                    ["response"] = captcha
                };

                var uri = new UriBuilder("https://www.google.com/recaptcha/api/siteverify");
                uri.AddQuery(nvc);

                var result = await _httpClient.GetAsJsonAsync<RecaptchaResponse>(uri.Uri, context.HttpContext.RequestAborted);

                //var result = await _httpClient.GetAsync<RecaptchaResponse>(
                //    new Uri("https://www.google.com/recaptcha/api/siteverify"), nvc,
                //    context.HttpContext.RequestAborted);

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

            private static string? ScanObject(ActionExecutingContext context)
            {
                foreach (var obj in context.ActionArguments.Values)
                {
                    switch (obj)
                    {
                        case CancellationToken _:
                            return null;
                    }

                    foreach (var property in obj.GetType().GetProperties())
                    {
                        var propValue = property.GetValue(obj, null);
                        if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string))
                        {
                            if (property.GetCustomAttribute(typeof(CaptchaAttribute)) != null)
                            {
                                return propValue?.ToString();
                            }
                        }
                        else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                        {
                            return null;
                        }
                        //else
                        //{
                        //    return ScanObject(propValue);
                        //}
                    }
                }

                return null;
            }
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local", Justification = "Json.net need to familiar with this")]
        public class RecaptchaResponse
        {
            public bool Success { get; set; }
        }
    }
}
