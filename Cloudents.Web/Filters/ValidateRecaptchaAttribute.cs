using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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

            public ValidateRecaptchaImpl(IConfiguration configuration, IRestClient httpClient)
            {
                _configuration = configuration;
                _httpClient = httpClient;
            }

            public override async Task OnActionExecutionAsync(ActionExecutingContext context,
                ActionExecutionDelegate next)
            {
                var captcha = context.HttpContext.Request.Form["captcha"];
                

                var secret = _configuration["GoogleReCaptcha:Secret"];


                var nvc = new NameValueCollection()
                {
                    ["secret"] = secret,
                    ["response"] = captcha
                };
                

                var result = await _httpClient.GetAsync<RecaptchaResponse>(
                    new Uri("https://www.google.com/recaptcha/api/siteverify"), nvc,
                    context.HttpContext.RequestAborted).ConfigureAwait(false);

                if (result == null)
                {
                        context.Result = new BadRequestResult();
                    return;
                }

                if (!result.Success)
                {
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
            }

            
        }

        public class RecaptchaResponse
        {
            public bool Success { get; set; }
        }
    }
}
