using Cloudents.Core.Entities.Db;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        internal const string Referral = "referral";
        private readonly List<IPAddress> _officeIps = new List<IPAddress>();


        public HomeController(IConfiguration configuration)
        {
            var ipsStr = configuration["Ips"];

            if (ipsStr == null) return;
            foreach (var ipStr in ipsStr.Split(','))
            {
                if (IPAddress.TryParse(ipStr, out var ip))
                {
                    _officeIps.Add(ip);
                }
            }
        }

        //[ResponseCache()]
        // we can't use that for now.
        // GET
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index(LocationQuery location, [FromQuery] bool? isNew,[FromQuery] string referral, [FromServices]IHostingEnvironment env)
        {
            if (!string.IsNullOrEmpty(referral))
            {
                //var base62 = new Base62(referral);
                TempData[Referral] = referral;
            }

            if (env.IsDevelopment())
            {
                return View();
            }

            if (env.IsStaging())
            {
                return View();
            }

            if (isNew.GetValueOrDefault(false))
            {
                return View();
            }
            var requestIp = HttpContext.Connection.GetIpAddress();
            if (_officeIps.Contains(requestIp))
            {
                return View();
            }
            if (string.Equals(location?.Address?.CountryCode, "il", StringComparison.InvariantCultureIgnoreCase))
            {
                return this.RedirectToOldSite();
            }
            return View();
        }

        [Route("logout")]
        public async Task<IActionResult> LogOutAsync([FromServices] SignInManager<User> signInManager)
        {
            await signInManager.SignOutAsync().ConfigureAwait(false);
            TempData.Clear();
            return Redirect("/");
        }
    }
}