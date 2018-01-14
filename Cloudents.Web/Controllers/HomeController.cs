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
            IPAddress.Parse("31.154.39.170")
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
            return View();
        }
    }
}