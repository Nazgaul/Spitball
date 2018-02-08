using System;
using System.Collections.Generic;
using System.Net;
using Cloudents.Core.Models;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Cloudents.Web.Controllers
{
    public class HomeController : Controller
    {
        public readonly List<IPAddress> OfficeIps = new List<IPAddress>();

        public HomeController( IConfiguration configuration)
        {
            var ipsStr = configuration["Ips"];

            if (ipsStr == null) return;
            foreach (var ipStr in ipsStr.Split(','))
            {
                if (IPAddress.TryParse(ipStr, out var ip))
                {
                    OfficeIps.Add(ip);
                }
            }
        }

        //[ResponseCache()]
        // we can't use that for now.
        // GET
       // [TypeFilter(typeof(IpToLocationActionFilter), Arguments = new object[] { "location" })]
        public IActionResult Index(Location location, [FromServices]IHostingEnvironment env)
        {
            if (string.Equals(env.EnvironmentName, "Development", StringComparison.InvariantCultureIgnoreCase))
            {
                return View();
            }

            if (string.Equals(env.EnvironmentName, "Staging", StringComparison.InvariantCultureIgnoreCase))
            {
                return View();
            }
            var requestIp = HttpContext.Connection.GetIpAddress();
            if (OfficeIps.Contains(requestIp))
            {
                return View();
            }
            if (!string.Equals(location?.CountryCode, "US", StringComparison.InvariantCultureIgnoreCase))
            {
                return this.RedirectToOldSite();
            }
            return View();
        }
    }
}