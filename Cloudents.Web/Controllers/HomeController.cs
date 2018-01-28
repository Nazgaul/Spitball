using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace Cloudents.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIpToLocation _ipToLocation;

        public readonly List<IPAddress> OfficeIps = new List<IPAddress>();

        public HomeController(IIpToLocation ipToLocation, IConfiguration configuration)
        {
            _ipToLocation = ipToLocation;
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
        public IActionResult Index(Location location)
        {
            var requestIp = HttpContext.Connection.GetIpAddress();
            if (OfficeIps.Contains(requestIp))
            {
                return View();
            }
            
            //var location = await _ipToLocation.GetAsync(requestIp, token).ConfigureAwait(false);

            if (!string.Equals(location?.CountryCode, "US", StringComparison.InvariantCultureIgnoreCase))
            {
                return this.RedirectToOldSite();
            }
            return View();
        }
    }

    public class TestModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            return Task.CompletedTask;
        }
    }
}