using System.Collections.Specialized;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Cloudents.Web.Filters
{
    public sealed class ValidateRecaptchaAttribule : TypeFilterAttribute
    {
        public ValidateRecaptchaAttribule() : base(typeof(ValidateRecaptchaImpl))
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

            public async Task OnActionExecutionAsync(ActionExecutingContext context,
                ActionExecutionDelegate next)
            {
                var captcha = context.HttpContext.Request.Form["g-recaptcha-response"];

                var secret = _configuration["GoogleReCaptcha:Secret"];


                var nvc = new NameValueCollection()
                {
                    ["secret"] = secret,
                    ["response"] = captcha
                };

                var result = await _httpClient.GetAsync<RecaptchaResponse>(
                    new System.Uri("https://www.google.com/recaptcha/api/siteverify"), nvc,
                    context.HttpContext.RequestAborted);

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

            private class RecaptchaResponse
            {
                [JsonConverter(typeof(YesNoConverter))]
                public bool Success { get; set; }
            }
        }
    }
}
