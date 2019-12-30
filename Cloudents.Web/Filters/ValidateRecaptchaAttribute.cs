using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Cloudents.Web.Filters
{


    public class ApiNotFoundFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Path.StartsWithSegments(new PathString("/api"),
                StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new NotFoundResult();
                return;
            }
            base.OnActionExecuting(context);
            // do something before the action executes
        }


    }


    public sealed class ValidateRecaptchaAttribute : TypeFilterAttribute
    {
        //private string SecretKey { get; }

        public ValidateRecaptchaAttribute(string secretKey) : base(typeof(ValidateRecaptchaImpl))
        {
            this.Arguments = new object[] { secretKey };
            //SecretKey = secretKey;
        }



        private class ValidateRecaptchaImpl : ActionFilterAttribute
        {
            private readonly string _secretKey;
            private readonly IRestClient _httpClient;
            private readonly IWebHostEnvironment _environment;

            public ValidateRecaptchaImpl(string secretKey, /*IConfiguration configuration,*/ IRestClient httpClient, IWebHostEnvironment environment)
            {
                //if (!string.IsNullOrEmpty(secretKey))
                //{
                _secretKey = secretKey;
                //}
                //else
                //{
                //    _secretKey= configuration["GoogleReCaptcha:Secret"];
                //}

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
                string captcha;
                context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(context.HttpContext.Request.Body))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var t = await JToken.ReadFromAsync(jsonTextReader);
                    captcha = t["captcha"]?.Value<string>();
                }

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
