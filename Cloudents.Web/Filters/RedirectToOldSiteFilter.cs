//using Cloudents.Web.Extensions;
//using Cloudents.Web.Services;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using System;
//using System.Net;
//using System.Threading.Tasks;

//namespace Cloudents.Web.Filters
//{
//    public class RedirectToOldSiteFilterAttribute : ActionFilterAttribute
//    {
//        private readonly IHostingEnvironment _environment;
//        private readonly IPAddress _officeIp = IPAddress.Parse("31.154.39.170");
//        private readonly ICountryProvider _countryProvider;

//        public RedirectToOldSiteFilterAttribute(IHostingEnvironment environment, ICountryProvider countryProvider)
//        {
//            _environment = environment;
//            _countryProvider = countryProvider;
//        }

//        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//        {
//            if (_environment.IsDevelopment())
//            {
//                await next();
//                return;

//            }

//            if (_environment.IsStaging())
//            {
//                await next();
//                return;
//            }
//            if (context.HttpContext.User.Identity.IsAuthenticated)
//            {
//                await next();
//                return;
//            }

//            var query = context.HttpContext.Request.Query["isNew"];
//            bool.TryParse(query.ToString(), out var isNew);
//            if (isNew)
//            {
//                await next();
//                return;
//            }

//            var requestIp = context.HttpContext.Connection.GetIpAddress();//.MapToIPv4().;

//            if (Equals(_officeIp, requestIp))
//            {
//                await next();
//                return;
//            }

//            var country = await _countryProvider.GetUserCountryAsync(context.HttpContext.RequestAborted);
//            if (string.Equals(country, "il", StringComparison.InvariantCultureIgnoreCase))
//            {
//                var path = context.HttpContext.Request.Path.Value.TrimEnd('/');
//                var uriBuilder = new UriBuilder
//                {
//                    Scheme = "https",
//                    Host = "heb.spitball.co",
//                    Path = path + "/",
//                    Query = context.HttpContext.Request.QueryString.Value
//                };
//                context.Result = new RedirectResult(uriBuilder.ToString());
//            }
//            else
//            {
//                await next();
//            }
//        }
//    }
//}
