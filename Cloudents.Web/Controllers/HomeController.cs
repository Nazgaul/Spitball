using Cloudents.Core.Entities.Db;
using Cloudents.Core.Extension;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Microsoft.Extensions.Configuration;

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

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index(
            [ModelBinder(typeof(CountryModelBinder))] string country,
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromQuery] bool? isNew, [FromQuery, CanBeNull] string referral, [FromServices]IHostingEnvironment env)
        {
            if (!string.IsNullOrEmpty(referral))
            {
                TempData[Referral] = referral;
            }

            if (userAgent != null && userAgent.Contains("linkedin", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.fbImage = ViewBag.imageSrc = "/images/3rdParty/linkedinShare.png";
            }

            ViewBag.country = country ?? "us";

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

            var requestIp = HttpContext.Connection.GetIpAddress();//.MapToIPv4().;
            


            if (_officeIps.Contains(requestIp))
            {
                return View();
            }
            if (string.Equals(country, "il", StringComparison.InvariantCultureIgnoreCase))
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