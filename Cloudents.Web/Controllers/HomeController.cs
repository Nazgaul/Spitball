using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIpToLocation _ipToLocation;

        public static readonly IPAddress[] OfficeIps = {
            IPAddress.Parse("31.154.39.170"),
            IPAddress.Parse("173.163.126.102"),
            IPAddress.Parse("174.59.52.138"),
            IPAddress.Parse("65.209.60.146"),
            IPAddress.Parse("74.66.78.189")
        };

        public HomeController(IIpToLocation ipToLocation)
        {
            _ipToLocation = ipToLocation;
        }

        //[ResponseCache()] 
        // we can't use that for now.
        // GET
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var requestIp = HttpContext.Connection.GetIpAddress();
            if (OfficeIps.Contains(requestIp))
            {
                return View();
            }

            var location = await _ipToLocation.GetAsync(requestIp, token).ConfigureAwait(false);

            if (!string.Equals(location.CountryCode, "US", StringComparison.InvariantCultureIgnoreCase))
            {
                return this.RedirectToOldSite();
            }
            return RedirectToRoute("Alex");
        }
    }
}