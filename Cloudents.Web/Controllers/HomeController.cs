using Cloudents.Core.Entities.Db;
using Cloudents.Core.Extension;
using Cloudents.Web.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        internal const string Referral = "referral";
        //private readonly List<IPAddress> _officeIps = new List<IPAddress>();


        //public HomeController(IConfiguration configuration)
        //{
        //    var ipsStr = configuration["Ips"];

        //    if (ipsStr == null) return;
        //    foreach (var ipStr in ipsStr.Split(','))
        //    {
        //        if (IPAddress.TryParse(ipStr, out var ip))
        //        {
        //            _officeIps.Add(ip);
        //        }
        //    }
        //}

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index(LocationQuery location,
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

            //if (env.IsDevelopment())
            //{
            //    return View();
            //}

            //if (env.IsStaging())
            //{
            //    return View();
            //}

            //if (isNew.GetValueOrDefault(false))
            //{
            //    return View();
            //}
            //var requestIp = HttpContext.Connection.GetIpAddress();
            //if (_officeIps.Contains(requestIp))
            //{
            //    return View();
            //}
            //if (string.Equals(location?.Address?.CountryCode, "il", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return this.RedirectToOldSite();
            //}
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