using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIpToLocation _ipToLocation;

        public static readonly IPAddress OfficeIp = IPAddress.Parse("31.154.39.170");

        public HomeController(IIpToLocation ipToLocation)
        {
            _ipToLocation = ipToLocation;
        }

        //[ResponseCache()] 
        // we can't use that for now.
        // GET
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var requestIp = this.HttpContext.Connection.GetIpAddress();
            if (Equals(requestIp, OfficeIp))
            {
                return View();
            }

            var location = await _ipToLocation.GetAsync(requestIp, token);

            if (!string.Equals(location.CountryCode, "US", StringComparison.InvariantCultureIgnoreCase))
            {

                var uriBuilder = new UriBuilder()
                {
                    Scheme = "https",
                    Host = "heb.spitball.co",
                    Path = HttpContext.Request.Path,
                    Query = HttpContext.Request.QueryString.Value
                };
                return Redirect(uriBuilder.ToString());
            }
            return RedirectToRoute("Alex");

            //return View();
        }
    }
}